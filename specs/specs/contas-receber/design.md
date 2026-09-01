# Design — Contas a Receber

Mesmo padrão de `contas-pagar/design.md`, com duas diferenças: FK obrigatória para `Cliente` (validada nos Handlers, mesmo padrão de `Veiculo`) e endpoint extra de listagem por cliente.

## Entidade (`Core/Domain/Entities/ContaReceber.cs`)

```csharp
public class ContaReceber
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataRecebimento { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
```

Sem propriedade de navegação para `Cliente`, mesmo padrão de `Veiculo`.

## Status (calculado, não persistido)

Reaproveita `StatusConta.Calcular(dataQuitacao, dataVencimento)` de `Core/Application/Common/StatusConta.cs` (criado em `contas-pagar`, se essa spec for implementada antes) — `Status = StatusConta.Calcular(conta.DataRecebimento, conta.DataVencimento)`, retornando "Paga"/"Pendente"/"Atrasada" (o rótulo genérico "Paga" serve tanto para pagamento quanto recebimento; não há um rótulo "Recebida" separado, para não duplicar a constante — ver observação de nomenclatura abaixo).

**Nota de nomenclatura:** o `spec.md` desta feature usa "Recebida" como rótulo de status, mas para reaproveitar `StatusConta` sem duplicar a classe, o valor efetivo retornado pela API é `"Paga"` mesmo em `ContaReceber` (mesma constante). Se essa diferença de rótulo for um problema para quem consome a API, criar uma constante `Recebida` separada é uma mudança pequena — não feita agora para não duplicar lógica antes de haver um consumidor real da API que exija o rótulo exato.

## Tabela / mapeamento (`ContaReceberConfiguration.cs`)

Tabela `contas_receber`, colunas em snake_case:

| Propriedade | Coluna | Observação |
|---|---|---|
| Id | id | |
| ClienteId | cliente_id | FK para `clientes.id`, `HasForeignKey` sem navegação, `OnDelete(DeleteBehavior.Restrict)` — mesmo padrão de `Veiculo` |
| Descricao | descricao | `HasMaxLength(200)` |
| Valor | valor | `HasColumnType("numeric(12,2)")` |
| DataVencimento | data_vencimento | |
| DataRecebimento | data_recebimento | nullable |
| DataCadastro | data_cadastro | |

## Migration

Nova migration `AddContasReceber`.

## DTO (`Core/Application/DTOs/ContaReceberDto.cs`)

```csharp
public class ContaReceberDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataRecebimento { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
```

## Commands / Queries (CQRS)

| Ação | Tipo | Arquivo |
|---|---|---|
| Criar | Command | `Commands/ContasReceber/CreateContaReceber/CreateContaReceberCommand.cs` |
| Atualizar | Command | `Commands/ContasReceber/UpdateContaReceber/UpdateContaReceberCommand.cs` |
| Remover | Command | `Commands/ContasReceber/DeleteContaReceber/DeleteContaReceberCommand.cs` |
| Listar todas | Query | `Queries/ContasReceber/GetAllContasReceber/GetAllContasReceberQuery.cs` |
| Buscar por id | Query | `Queries/ContasReceber/GetContaReceberById/GetContaReceberByIdQuery.cs` |
| Listar por cliente | Query | `Queries/ContasReceber/GetContasReceberByClienteId/GetContasReceberByClienteIdQuery.cs` |

Mesmo padrão de `Veiculo` para validação de `ClienteId`: `CreateContaReceberHandler` injeta `IClienteRepository`, retorna `ContaReceberDto?` (`null` = cliente inválido → `400`). `UpdateContaReceberHandler` retorna o mesmo enum reaproveitado de `Veiculo`? **Não** — cada feature tem seu próprio enum de resultado (`UpdateContaReceberResult`), para não criar acoplamento entre features via um enum compartilhado que não tem relação de domínio:

```csharp
public enum UpdateContaReceberResult { Success, ContaNotFound, ClienteInvalido }
```

`GetContasReceberByClienteIdQuery`: mesmo comportamento de `GetVeiculosByClienteIdQuery` — não valida existência do cliente, só filtra; lista vazia serve tanto para "cliente sem contas" quanto "cliente inexistente".

## Repository

- Interface: `Core/Application/Interfaces/Repositories/IContaReceberRepository.cs`
- Implementação: `Infrastructure/Persistence/Repositories/ContaReceberRepository.cs`
- Métodos: `GetAllAsync`, `GetByIdAsync`, `GetByClienteIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`.

## Endpoints (`API/Controllers/ContasReceberController.cs`)

| Método | Rota | Command/Query |
|---|---|---|
| GET | `/api/ContasReceber` | `GetAllContasReceberQuery` |
| GET | `/api/ContasReceber/{id}` | `GetContaReceberByIdQuery` |
| GET | `/api/ContasReceber/cliente/{clienteId}` | `GetContasReceberByClienteIdQuery` |
| POST | `/api/ContasReceber` | `CreateContaReceberCommand` |
| PUT | `/api/ContasReceber/{id}` | `UpdateContaReceberCommand` |
| DELETE | `/api/ContasReceber/{id}` | `DeleteContaReceberCommand` |

Mesmo padrão de `VeiculosController` para o mapeamento do enum de update (`Success`→204, `ContaNotFound`→404, `ClienteInvalido`→400) e do `null` no create (→400).

## Registro (`Program.cs` / `AppDbContext.cs`)

- `builder.Services.AddScoped<IContaReceberRepository, ContaReceberRepository>();`
- `AppDbContext`: `public DbSet<ContaReceber> ContasReceber => Set<ContaReceber>();`
