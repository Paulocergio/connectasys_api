# Design — Contas a Pagar

## Entidade (`Core/Domain/Entities/ContaPagar.cs`)

```csharp
public class ContaPagar
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Fornecedor { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
```

`Id` é `int` (identity, gerado pelo banco) — mesmo padrão de `Cliente`/`Veiculo`. `DataPagamento` é `DateTime?`: `null` = pendente/atrasada.

## Status (calculado, não persistido)

Não vira coluna — calculado sob demanda, tanto no `ContaPagarDto` (ao montar a resposta) quanto em qualquer outro lugar que precise dele. Para não duplicar a lógica entre Handlers, fica num método estático simples:

```csharp
public static class StatusConta
{
    public const string Paga = "Paga";
    public const string Pendente = "Pendente";
    public const string Atrasada = "Atrasada";

    public static string Calcular(DateTime? dataQuitacao, DateTime dataVencimento) =>
        dataQuitacao is not null
            ? Paga
            : DateTime.UtcNow.Date > dataVencimento.Date
                ? Atrasada
                : Pendente;
}
```

Arquivo novo: `Core/Application/Common/StatusConta.cs` (pasta `Common` nova, criada sob demanda — a constitution permite). Compartilhado entre `ContaPagar` e `ContaReceber`: mesmo texto "Paga"/"Pendente"/"Atrasada" nos dois DTOs, só muda o rótulo que o Handler usa ao montar o DTO (`Status = StatusConta.Calcular(...)`), sem duplicar a regra de data.

## Tabela / mapeamento (`ContaPagarConfiguration.cs`)

Tabela `contas_pagar`, colunas em snake_case:

| Propriedade | Coluna | Observação |
|---|---|---|
| Id | id | |
| Descricao | descricao | `HasMaxLength(200)` |
| Fornecedor | fornecedor | `HasMaxLength(150)` |
| Valor | valor | `HasColumnType("numeric(12,2)")` |
| DataVencimento | data_vencimento | |
| DataPagamento | data_pagamento | nullable |
| DataCadastro | data_cadastro | |

```csharp
builder.ToTable("contas_pagar");
builder.HasKey(c => c.Id);

builder.Property(c => c.Id).HasColumnName("id");
builder.Property(c => c.Descricao).HasColumnName("descricao").HasMaxLength(200);
builder.Property(c => c.Fornecedor).HasColumnName("fornecedor").HasMaxLength(150);
builder.Property(c => c.Valor).HasColumnName("valor").HasColumnType("numeric(12,2)");
builder.Property(c => c.DataVencimento).HasColumnName("data_vencimento");
builder.Property(c => c.DataPagamento).HasColumnName("data_pagamento");
builder.Property(c => c.DataCadastro).HasColumnName("data_cadastro");
```

## Migration

Nova migration `AddContasPagar` (`dotnet ef migrations add AddContasPagar`).

## DTO (`Core/Application/DTOs/ContaPagarDto.cs`)

```csharp
public class ContaPagarDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Fornecedor { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
```

## Commands / Queries (CQRS)

| Ação | Tipo | Arquivo |
|---|---|---|
| Criar | Command | `Commands/ContasPagar/CreateContaPagar/CreateContaPagarCommand.cs` |
| Atualizar | Command | `Commands/ContasPagar/UpdateContaPagar/UpdateContaPagarCommand.cs` |
| Remover | Command | `Commands/ContasPagar/DeleteContaPagar/DeleteContaPagarCommand.cs` |
| Listar todas | Query | `Queries/ContasPagar/GetAllContasPagar/GetAllContasPagarQuery.cs` |
| Buscar por id | Query | `Queries/ContasPagar/GetContaPagarById/GetContaPagarByIdQuery.cs` |

`CreateContaPagarCommand` não tem `DataPagamento` (spec: nasce sempre pendente). `UpdateContaPagarCommand` tem `DateTime? DataPagamento` — igual em espírito ao `Senha` opcional de `UpdateUsuarioCommand`, mas aqui o campo é sempre re-atribuído (inclusive para `null`, caso o atendente precise "desmarcar" um pagamento lançado por engano) em vez de "só atualiza se enviado" — porque não há como distinguir "omitido" de "explicitamente nulo" num tipo já nullable sem um DTO de patch, e o caso de uso aqui é update completo (PUT), não patch parcial.

Sem Handler de "validação de cliente" nesta feature — `ContaPagar` não tem FK.

## Repository

- Interface: `Core/Application/Interfaces/Repositories/IContaPagarRepository.cs`
- Implementação: `Infrastructure/Persistence/Repositories/ContaPagarRepository.cs`
- Métodos: `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` — mesmo padrão de `IClienteRepository`.

## Endpoints (`API/Controllers/ContasPagarController.cs`)

| Método | Rota | Command/Query |
|---|---|---|
| GET | `/api/ContasPagar` | `GetAllContasPagarQuery` |
| GET | `/api/ContasPagar/{id}` | `GetContaPagarByIdQuery` |
| POST | `/api/ContasPagar` | `CreateContaPagarCommand` |
| PUT | `/api/ContasPagar/{id}` | `UpdateContaPagarCommand` |
| DELETE | `/api/ContasPagar/{id}` | `DeleteContaPagarCommand` |

Mesmo padrão de `ClientesController` — sem validação de FK, então `Update`/`Create` só retornam `bool`/DTO (sem enum de resultado como em `Veiculo`).

## Registro (`Program.cs` / `AppDbContext.cs`)

- `builder.Services.AddScoped<IContaPagarRepository, ContaPagarRepository>();`
- `AppDbContext`: `public DbSet<ContaPagar> ContasPagar => Set<ContaPagar>();`
