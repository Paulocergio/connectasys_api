# Design — Usuários

## Hash de senha — decisão técnica

**Onde o hash acontece:** num serviço dedicado `IPasswordHasher` (Core) / `PasswordHasher` (Infrastructure), injetado nos Handlers de `CreateUsuario` e `UpdateUsuario` — não direto no Handler com chamada estática ao BCrypt, e não numa camada de "serviço de domínio" separada.

Motivo: o projeto já usa o padrão interface-em-`Core`/implementação-em-`Infrastructure` para o Repository (`IUsuarioRepository` / `UsuarioRepository`). Hashing de senha é um detalhe de infraestrutura (depende do pacote `BCrypt.Net-Next`) — colocar a implementação em `Infrastructure` mantém `Core` livre de dependência de pacote externo, seguindo a regra da constitution de que `Core` não depende de mais nada. Os Handlers continuam sendo o lugar onde a lógica de orquestração do caso de uso mora (mesmo padrão CQRS já usado), só que agora orquestram também a chamada ao hasher.

### Interface (`Core/Application/Interfaces/Services/IPasswordHasher.cs`)

```csharp
namespace connectasys_api.Core.Application.Interfaces.Services
{
    public interface IPasswordHasher
    {
        string Hash(string senha);
        bool Verify(string senha, string hash);
    }
}
```

`Verify` não é usado nesta feature (login fica fora de escopo), mas entra na interface agora porque faz parte do mesmo conceito e evita reabrir a interface quando o login for implementado.

Pasta nova: `Core/Application/Interfaces/Services/` (hoje só existe `Interfaces/Repositories/`) — criada sob demanda, conforme a constitution permite.

### Implementação (`Infrastructure/Security/PasswordHasher.cs`)

```csharp
using connectasys_api.Core.Application.Interfaces.Services;

namespace connectasys_api.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string senha) => BCrypt.Net.BCrypt.HashPassword(senha);

        public bool Verify(string senha, string hash) => BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}
```

Pasta nova: `Infrastructure/Security/`.

### Pacote

Adicionar `BCrypt.Net-Next` ao `Infrastructure.csproj` (não ao `Core.csproj` — mantém a regra de dependência da constitution).

### Registro em DI (`API/Program.cs`)

```csharp
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
```

`Singleton` porque o hasher é stateless (sem estado por requisição) — diferente do `IUsuarioRepository`, que é `Scoped` porque depende do `DbContext`.

## Entidade (`Core/Domain/Entities/Usuario.cs`)

```csharp
public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public DateTime DataCriacaoUtc { get; set; } = DateTime.UtcNow;
}
```

`Id` é `Guid` (gerado em código com `Guid.NewGuid()` no Handler de criação) — diferente de `Cliente`, que usa `int` identity do banco.

`SenhaHash` é a única adição — a entidade nunca tem uma propriedade `Senha` em texto puro; o texto puro só existe de passagem, dentro do Handler, entre receber o Command e chamar `IPasswordHasher.Hash(...)`.

## Tabela / mapeamento (`UsuarioConfiguration.cs`)

Tabela `usuarios`, colunas em snake_case:

| Propriedade | Coluna | Observação |
|---|---|---|
| Id | id | |
| Nome | nome | |
| Email | email | |
| Role | role | |
| Telefone | telefone | |
| SenhaHash | senha_hash | `HasMaxLength(60)` — hash do BCrypt tem tamanho fixo de 60 caracteres |
| DataCriacaoUtc | data_criacao_utc | |

```csharp
builder.Property(u => u.SenhaHash).HasColumnName("senha_hash").HasMaxLength(60);
```

## Migration

**Importante:** ao contrário do que se supunha inicialmente, a coluna `senha_hash` **não existe** na tabela `usuarios` atual — ela foi removida quando a tabela foi recriada do zero (migration `RecreateUsuarios`) numa versão anterior da spec. Portanto esta feature precisa de uma **migration nova** (`dotnet ef migrations add AddSenhaHashToUsuarios`), não apenas de mapear uma coluna já existente.

- Nova coluna: `senha_hash character varying(60) not null`.
- Como o ambiente é local/dev e não há dado de produção (constitution, seção 6), não é necessário planejar backfill: se já houver usuários cadastrados no banco local, eles precisam ser recriados (deletados e recadastrados) após aplicar a migration, já que não têm senha. Isso é aceitável nesta fase e será registrado como task explícita.
- Migration a ser gerada e aplicada localmente via `dotnet ef database update`, seguindo o mesmo fluxo já usado para `InitialCreate`/`RecreateUsuarios`.

## DTO (`Core/Application/DTOs/UsuarioDto.cs`)

**Sem alterações.** O DTO já não tem nenhum campo de senha/hash — continua exatamente como está, o que já satisfaz o critério de aceite "senha/hash nunca aparecem em nenhuma resposta da API". Nenhum Handler deve nunca preencher um DTO com `SenhaHash`.

## Commands / Queries (CQRS)

| Ação | Tipo | Arquivo | Mudança |
|---|---|---|---|
| Criar | Command | `Commands/Usuarios/CreateUsuario/CreateUsuarioCommand.cs` | + campo `Senha` (obrigatório) |
| Atualizar | Command | `Commands/Usuarios/UpdateUsuario/UpdateUsuarioCommand.cs` | + campo `Senha` (opcional/nullable) |
| Remover | Command | `Commands/Usuarios/DeleteUsuario/DeleteUsuarioCommand.cs` | sem mudança |
| Listar todos | Query | `Queries/Usuarios/GetAllUsuarios/GetAllUsuariosQuery.cs` | sem mudança |
| Buscar por id | Query | `Queries/Usuarios/GetUsuarioById/GetUsuarioByIdQuery.cs` | sem mudança |

### `CreateUsuarioCommand`

```csharp
public class CreateUsuarioCommand : IRequest<UsuarioDto>
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
```

### `CreateUsuarioHandler`

Injeta `IPasswordHasher` além do `IUsuarioRepository` já existente. Preenche `SenhaHash = _passwordHasher.Hash(request.Senha)` ao montar a entidade — `request.Senha` (texto puro) nunca é atribuída a nenhuma propriedade da entidade nem retornada no DTO.

### `UpdateUsuarioCommand`

```csharp
public class UpdateUsuarioCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Senha { get; set; }
}
```

`Senha` é `string?` — só existe na entrada quando o admin quer trocar a senha.

### `UpdateUsuarioHandler`

Depois de carregar `usuario` via `_repository.GetByIdAsync(request.Id)`:

```csharp
if (!string.IsNullOrWhiteSpace(request.Senha))
{
    usuario.SenhaHash = _passwordHasher.Hash(request.Senha);
}
```

Se `Senha` vier nula/vazia, `SenhaHash` não é tocado — a senha atual permanece válida.

## Repository

- Interface: `Core/Application/Interfaces/Repositories/IUsuarioRepository.cs`
- Implementação: `Infrastructure/Persistence/Repositories/UsuarioRepository.cs`
- Métodos: `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`

Sem mudanças — `SenhaHash` é só mais uma propriedade da entidade que o EF já persiste via `UpdateAsync`/`AddAsync` existentes.

## Endpoints (`API/Controllers/UsuariosController.cs`)

| Método | Rota | Command/Query |
|---|---|---|
| GET | `/api/Usuarios` | `GetAllUsuariosQuery` |
| GET | `/api/Usuarios/{id}` | `GetUsuarioByIdQuery` |
| POST | `/api/Usuarios` | `CreateUsuarioCommand` |
| PUT | `/api/Usuarios/{id}` | `UpdateUsuarioCommand` |
| DELETE | `/api/Usuarios/{id}` | `DeleteUsuarioCommand` |

Sem mudanças de rota — `Senha` chega no corpo dos mesmos `POST`/`PUT` que já existem. Controller depende apenas de `IMediator`, segue o mesmo padrão de `ClientesController`.
