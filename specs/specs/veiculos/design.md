# Design — Veículos

## Entidade (`Core/Domain/Entities/Veiculo.cs`)

```csharp
public class Veiculo
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Cor { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
```

`Id` é `int` (identity, gerado pelo banco) — mesmo padrão de `Cliente`, já que `Veiculo` é uma entidade simples sem dado sensível, diferente de `Usuario` (`Guid`).

Sem propriedade de navegação (`Cliente Cliente { get; set; }`) por enquanto — o projeto não usa navegação EF em nenhuma entidade existente ainda (`Cliente`/`Usuario` não têm relacionamentos). `ClienteId` é só uma FK simples; a validação de existência do cliente acontece no Handler via `IClienteRepository`, não via constraint de navegação do EF.

## Tabela / mapeamento (`VeiculoConfiguration.cs`)

Tabela `veiculos`, colunas em snake_case:

| Propriedade | Coluna | Observação |
|---|---|---|
| Id | id | |
| ClienteId | cliente_id | FK para `clientes.id`, `HasForeignKey` sem propriedade de navegação |
| Placa | placa | `HasMaxLength(10)` |
| Marca | marca | `HasMaxLength(50)` |
| Modelo | modelo | `HasMaxLength(50)` |
| Ano | ano | |
| Cor | cor | `HasMaxLength(30)` |
| DataCadastro | data_cadastro | |

```csharp
builder.ToTable("veiculos");
builder.HasKey(v => v.Id);

builder.Property(v => v.Id).HasColumnName("id");
builder.Property(v => v.ClienteId).HasColumnName("cliente_id");
builder.Property(v => v.Placa).HasColumnName("placa").HasMaxLength(10);
builder.Property(v => v.Marca).HasColumnName("marca").HasMaxLength(50);
builder.Property(v => v.Modelo).HasColumnName("modelo").HasMaxLength(50);
builder.Property(v => v.Ano).HasColumnName("ano");
builder.Property(v => v.Cor).HasColumnName("cor").HasMaxLength(30);
builder.Property(v => v.DataCadastro).HasColumnName("data_cadastro");

builder.HasOne<Cliente>()
    .WithMany()
    .HasForeignKey(v => v.ClienteId)
    .OnDelete(DeleteBehavior.Restrict);
```

`OnDelete(DeleteBehavior.Restrict)`: não permite apagar um `Cliente` que ainda tenha veículos vinculados (evita órfãos silenciosos) — não há feature de exclusão em cascata prevista.

## Migration

Nova migration `AddVeiculos` (`dotnet ef migrations add AddVeiculos`), criando a tabela `veiculos` do zero com a FK para `clientes`.

## DTO (`Core/Application/DTOs/VeiculoDto.cs`)

```csharp
public class VeiculoDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Cor { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
```

Espelho direto da entidade — mesmo padrão de `ClienteDto`.

## Commands / Queries (CQRS)

| Ação | Tipo | Arquivo |
|---|---|---|
| Criar | Command | `Commands/Veiculos/CreateVeiculo/CreateVeiculoCommand.cs` |
| Atualizar | Command | `Commands/Veiculos/UpdateVeiculo/UpdateVeiculoCommand.cs` |
| Remover | Command | `Commands/Veiculos/DeleteVeiculo/DeleteVeiculoCommand.cs` |
| Listar todos | Query | `Queries/Veiculos/GetAllVeiculos/GetAllVeiculosQuery.cs` |
| Buscar por id | Query | `Queries/Veiculos/GetVeiculoById/GetVeiculoByIdQuery.cs` |
| Listar por cliente | Query | `Queries/Veiculos/GetVeiculosByClienteId/GetVeiculosByClienteIdQuery.cs` |

### Validação de `ClienteId`

`CreateVeiculoHandler` e `UpdateVeiculoHandler` injetam `IClienteRepository` (além de `IVeiculoRepository`) e chamam `GetByIdAsync(request.ClienteId)` antes de persistir. Se retornar `null`, o Handler lança `InvalidOperationException` (ou similar) capturada no Controller para retornar `400 Bad Request` — não há um padrão de exceptions customizadas no projeto ainda, então a checagem acontece diretamente no Controller: o Handler retorna `null` (via um tipo de retorno anulável, mesmo padrão de `Get...ById`) para sinalizar "cliente inválido", e o Controller decide `400` vs `201`/`204`.

Concretamente: `CreateVeiculoHandler` retorna `VeiculoDto?` (`null` = `ClienteId` inválido). `UpdateVeiculoHandler` já retorna `bool`; passa a existir uma distinção de motivo de falha (veículo não encontrado vs cliente inválido) resolvida com um enum de resultado simples:

```csharp
public enum UpdateVeiculoResult { Success, VeiculoNotFound, ClienteInvalido }
```

`UpdateVeiculoHandler : IRequestHandler<UpdateVeiculoCommand, UpdateVeiculoResult>`. Controller mapeia: `Success` → `204`, `VeiculoNotFound` → `404`, `ClienteInvalido` → `400`.

### `GetVeiculosByClienteIdQuery`

```csharp
public class GetVeiculosByClienteIdQuery : IRequest<List<VeiculoDto>>
{
    public int ClienteId { get; set; }
}
```

Não valida se o `ClienteId` existe — apenas filtra `IVeiculoRepository.GetByClienteIdAsync(clienteId)`, que retorna lista vazia tanto para "cliente existe mas sem veículos" quanto para "cliente não existe" (comportamento definido no `spec.md`).

## Repository

- Interface: `Core/Application/Interfaces/Repositories/IVeiculoRepository.cs`
- Implementação: `Infrastructure/Persistence/Repositories/VeiculoRepository.cs`
- Métodos: `GetAllAsync`, `GetByIdAsync`, `GetByClienteIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`

```csharp
public interface IVeiculoRepository
{
    Task<List<Veiculo>> GetAllAsync();
    Task<Veiculo?> GetByIdAsync(int id);
    Task<List<Veiculo>> GetByClienteIdAsync(int clienteId);
    Task AddAsync(Veiculo veiculo);
    Task UpdateAsync(Veiculo veiculo);
    Task DeleteAsync(Veiculo veiculo);
}
```

## Endpoints (`API/Controllers/VeiculosController.cs`)

| Método | Rota | Command/Query |
|---|---|---|
| GET | `/api/Veiculos` | `GetAllVeiculosQuery` |
| GET | `/api/Veiculos/{id}` | `GetVeiculoByIdQuery` |
| GET | `/api/Veiculos/cliente/{clienteId}` | `GetVeiculosByClienteIdQuery` |
| POST | `/api/Veiculos` | `CreateVeiculoCommand` |
| PUT | `/api/Veiculos/{id}` | `UpdateVeiculoCommand` |
| DELETE | `/api/Veiculos/{id}` | `DeleteVeiculoCommand` |

Controller depende apenas de `IMediator`, mesmo padrão de `ClientesController`/`UsuariosController`.

## Registro (`Program.cs` / `AppDbContext.cs`)

- `builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();`
- `AppDbContext`: `public DbSet<Veiculo> Veiculos => Set<Veiculo>();`
