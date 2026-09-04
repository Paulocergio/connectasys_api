# Design — Autenticação (Login)

## Geração do token — decisão técnica

Mesmo padrão já usado para hash de senha ([usuarios/design.md](../usuarios/design.md)): interface em `Core`, implementação em `Infrastructure`, injetada no Handler.

### Interface (`Core/Application/Interfaces/Services/ITokenService.cs`)

```csharp
namespace connectasys_api.Core.Application.Interfaces.Services
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiraEmUtc) GerarToken(Guid usuarioId, string email, string nome, string role);
    }
}
```

### Implementação (`Infrastructure/Security/TokenService.cs`)

JWT assinado com HMAC-SHA256 (`SymmetricSecurityKey` + `SigningCredentials`), usando `System.IdentityModel.Tokens.Jwt`. Claims: `Sub` (usuarioId), `Email`, `Name` (nome), `ClaimTypes.Role` (role). Expiração configurável (`Jwt:ExpiresMinutes`), lida via `IConfiguration` injetado no construtor.

Pacote novo: `Microsoft.AspNetCore.Authentication.JwtBearer` (traz `System.IdentityModel.Tokens.Jwt` como dependência transitiva) — adicionado ao `Infrastructure.csproj`, seguindo a mesma regra da constitution (Core livre de dependência de pacote externo).

### Registro em DI (`API/Program.cs`)

```csharp
builder.Services.AddSingleton<ITokenService, TokenService>();
```

`Singleton`: sem estado por requisição, só lê config — mesmo raciocínio do `IPasswordHasher`.

### Configuração (`appsettings.json`)

Nova seção, no mesmo arquivo onde já está a connection string (padrão local já em uso no projeto — segredo real em `appsettings.json`, não em variável de ambiente, por ora):

```json
"Jwt": {
  "Key": "chave-secreta-local-apenas-para-desenvolvimento-trocar-em-producao",
  "Issuer": "ConnectaSysApi",
  "Audience": "ConnectaSysApi",
  "ExpiresMinutes": 60
}
```

### Middleware de autenticação (`API/Program.cs`)

Hoje o `Program.cs` já chama `app.UseAuthorization()` mas nunca chamou `app.UseAuthentication()` (não havia autenticação nenhuma). Esta spec adiciona:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
```

e, antes de `app.UseAuthorization()`:

```csharp
app.UseAuthentication();
```

Nenhum endpoint existente ganha `[Authorize]` nesta spec (fora de escopo — ver `spec.md`); o middleware fica pronto para uso futuro.

## DTO (`Core/Application/DTOs/LoginResponseDto.cs`)

```csharp
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEmUtc { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
```

## Command (CQRS)

Login é modelado como `Command` (ação de entrada no sistema), mesmo não alterando dados — segue o mesmo raciocínio de nomenclatura de ação usado em `CreateUsuario`.

### `LoginCommand` (`Commands/Auth/Login/LoginCommand.cs`)

```csharp
public class LoginCommand : IRequest<LoginResponseDto?>
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
```

Retorna `null` quando as credenciais são inválidas (mesmo padrão de `GetUsuarioByIdHandler` retornando `null` para "não encontrado") — o Controller traduz `null` em `401 Unauthorized`.

### `LoginHandler` (`Commands/Auth/Login/LoginHandler.cs`)

1. Busca usuário por email via `IUsuarioRepository.GetByEmailAsync(request.Email)`.
2. Se não encontrado → retorna `null`.
3. Se encontrado, verifica senha via `IPasswordHasher.Verify(request.Senha, usuario.SenhaHash)`.
4. Se inválida → retorna `null`.
5. Se válida, chama `ITokenService.GerarToken(...)` e monta o `LoginResponseDto`.

Injeta `IUsuarioRepository`, `IPasswordHasher` (já existentes) e `ITokenService` (novo).

## Repository — novo método

`IUsuarioRepository` ganha:

```csharp
Task<Usuario?> GetByEmailAsync(string email);
```

Implementado em `UsuarioRepository`:

```csharp
public async Task<Usuario?> GetByEmailAsync(string email) =>
    await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
```

## Endpoint (`API/Controllers/AuthController.cs`, novo)

Controller novo (não faz sentido colocar login dentro de `UsuariosController`, que é CRUD de gestão de equipe — login é uma ação de sistema separada).

| Método | Rota | Command | Resposta |
|---|---|---|---|
| POST | `/api/Auth/login` | `LoginCommand` | `200 OK` com `LoginResponseDto`, ou `401 Unauthorized` |

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return result is null ? Unauthorized(new { message = "Email ou senha inválidos" }) : Ok(result);
    }
}
```
