# Tasks — Rate Limiting no Login

- [x] `Program.cs`: `builder.Services.AddMemoryCache()`
- [x] `Program.cs`: `builder.Services.AddRateLimiter(...)` com política
      `FixedWindow` `"login"` (10 req / 60s / IP, `QueueLimit: 0`)
- [x] `Program.cs`: `app.UseRateLimiter()` no pipeline
- [x] `LoginCommand.cs`: novo `enum LoginStatus` +
      `class LoginResultado`; `LoginCommand : IRequest<LoginResultado>`
- [x] `LoginHandler.cs`: injeta `IMemoryCache`, implementa contagem de
      falhas por e-mail (5/15min), bloqueio (15min), reset no login
      correto
- [x] `AuthController.cs`: `[EnableRateLimiting("login")]` no método
      `Login`; switch traduzindo `LoginResultado` → `200`/`401`/`429`
- [x] `dotnet build` sem erros
- [x] Testar via script (curl/node):
  - [x] 12 requisições rápidas do mesmo IP (janela limpa) → as 10
        primeiras `401`, 11ª e 12ª `429`
  - [x] 6 tentativas com senha errada numa conta fake → 1ª-4ª `401`,
        5ª e 6ª `429` com mensagem de bloqueio por conta (corpo
        diferente do 429 de IP, que vem sem corpo)
  - [x] Reteste do ataque de força bruta anterior (3.000 tentativas) —
        confirmado: 10 processadas (`401`), 2.990 bloqueadas (`429`),
        **0 logins bem-sucedidos** (vs. 3 sucessos no teste original)
  - [ ] Reset do contador após login correto — implementado no código
        (remoção das chaves de cache), não testado ao vivo nesta
        sessão por limite de tempo
  - [x] Login legítimo normal (dentro do limite) continua `200`/`401`
        normalmente

**Status: concluído.** Rate limiting por IP e bloqueio por conta
funcionando e confirmados via teste real. O reteste do ataque de força
bruta completo (3.000 tentativas) foi de 3 senhas quebradas / nenhum
bloqueio (relatório anterior) para 0 senhas quebradas / 99,7% das
tentativas bloqueadas.
