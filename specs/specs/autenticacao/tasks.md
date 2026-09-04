# Tasks — Autenticação (Login)

- [x] Adicionar pacote `Microsoft.AspNetCore.Authentication.JwtBearer` ao `Infrastructure.csproj`
- [x] Adicionar seção `Jwt` (`Key`, `Issuer`, `Audience`, `ExpiresMinutes`) ao `appsettings.json`
- [x] Criar `ITokenService` em `Core/Application/Interfaces/Services/ITokenService.cs`
- [x] Criar `TokenService` em `Infrastructure/Security/TokenService.cs`
- [x] Adicionar `GetByEmailAsync` a `IUsuarioRepository` e implementar em `UsuarioRepository`
- [x] Criar `LoginResponseDto` em `Core/Application/DTOs/LoginResponseDto.cs`
- [x] Criar `LoginCommand` + `LoginHandler` em `Commands/Auth/Login/`
- [x] Criar `AuthController` com `POST /api/Auth/login`
- [x] Registrar `ITokenService` no `Program.cs` (`AddSingleton`)
- [x] Configurar `AddAuthentication().AddJwtBearer(...)` e `app.UseAuthentication()` no `Program.cs`
- [x] Build da solução (`dotnet build`) sem erros
- [x] Testar via API real (curl contra a API rodando localmente + Postgres):
  - [x] Login com email/senha corretos → `200 OK` com token
  - [x] Login com email inexistente → `401 Unauthorized`
  - [x] Login com senha errada → `401 Unauthorized`
  - [x] Decodificar o token e conferir claims (sub, email, name, role, exp, iss, aud) — presentes; `name`/`role` aparecem com os URIs longos padrão do `ClaimTypes` do .NET, não `"name"`/`"role"` curtos (comportamento padrão do `JwtSecurityTokenHandler`, aceitável para o critério de aceite)

**Status: concluído.** Testado end-to-end (usuário de teste criado, login testado nos 3 cenários, usuário removido depois).
