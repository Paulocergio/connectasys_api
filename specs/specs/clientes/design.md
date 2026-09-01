# Design — Clientes

## Entidade (`Core/Domain/Entities/Cliente.cs`)

```csharp
public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
```

`Id` é `int` (identity, gerado pelo banco) — diferente de `Usuario`, que usa `Guid`.

## Tabela / mapeamento (`ClienteConfiguration.cs`)

Tabela `clientes`, colunas em snake_case:

| Propriedade | Coluna |
|---|---|
| Id | id |
| Nome | nome |
| Email | email |
| Telefone | telefone |
| DataCadastro | data_cadastro |

## DTO (`Core/Application/DTOs/ClienteDto.cs`)

Mesmos campos da entidade — não há dado sensível a esconder em `Cliente`, então o DTO é um espelho direto.

## Commands / Queries (CQRS)

| Ação | Tipo | Arquivo |
|---|---|---|
| Criar | Command | `Commands/Clientes/CreateCliente/CreateClienteCommand.cs` |
| Atualizar | Command | `Commands/Clientes/UpdateCliente/UpdateClienteCommand.cs` |
| Remover | Command | `Commands/Clientes/DeleteCliente/DeleteClienteCommand.cs` |
| Listar todos | Query | `Queries/Clientes/GetAllClientes/GetAllClientesQuery.cs` |
| Buscar por id | Query | `Queries/Clientes/GetClienteById/GetClienteByIdQuery.cs` |

Cada um com seu `Handler` correspondente na mesma pasta.

## Repository

- Interface: `Core/Application/Interfaces/Repositories/IClienteRepository.cs`
- Implementação: `Infrastructure/Persistence/Repositories/ClienteRepository.cs`
- Métodos: `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`

## Endpoints (`API/Controllers/ClientesController.cs`)

| Método | Rota | Command/Query |
|---|---|---|
| GET | `/api/Clientes` | `GetAllClientesQuery` |
| GET | `/api/Clientes/{id}` | `GetClienteByIdQuery` |
| POST | `/api/Clientes` | `CreateClienteCommand` |
| PUT | `/api/Clientes/{id}` | `UpdateClienteCommand` |
| DELETE | `/api/Clientes/{id}` | `DeleteClienteCommand` |

Controller depende apenas de `IMediator` — nenhum acesso direto ao `AppDbContext`.
