# Design — Rate Limiting no Login

## Abordagem

Duas camadas independentes, ambas só no endpoint de login:

### 1. Limite por IP — middleware nativo do ASP.NET Core

`Microsoft.AspNetCore.RateLimiting` (nativo desde o .NET 7, sem
dependência nova). Política `FixedWindow`, nomeada `"login"`:

- `PermitLimit`: 10
- `Window`: 60 segundos
- `QueueLimit`: 0 (excedeu, rejeita na hora — não fila)
- Particionado por IP remoto (`HttpContext.Connection.RemoteIpAddress`)

Registrado em `Program.cs` (`AddRateLimiter` + `app.UseRateLimiter()`)
e aplicado só na action de login via `[EnableRateLimiting("login")]`
em `AuthController.Login`.

### 2. Bloqueio por conta — `IMemoryCache`

`LoginHandler` passa a receber `IMemoryCache` (registrado com
`builder.Services.AddMemoryCache()`) e rastrear tentativas falhas por
e-mail:

- Chave `login-tentativas:{email}` → contador, expira em 15 min
  (janela de acúmulo)
- Ao atingir 5 falhas: grava `login-bloqueado:{email}` → `true`,
  expira em 15 min (duração do bloqueio)
- Login correto: remove as duas chaves daquele e-mail

`LoginCommand` deixa de retornar só `LoginResponseDto?` e passa a
retornar um resultado com 3 estados:

```csharp
public enum LoginStatus { Sucesso, Invalido, Bloqueado }

public class LoginResultado
{
    public LoginStatus Status { get; set; }
    public LoginResponseDto? Resposta { get; set; }
}
```

`AuthController.Login` traduz:

| Status | HTTP |
|---|---|
| `Sucesso` | `200 OK` com o `LoginResponseDto` |
| `Invalido` | `401 Unauthorized` (mesma mensagem de hoje) |
| `Bloqueado` | `429 Too Many Requests` |

## Arquivos alterados

- `src/API/Program.cs` — `AddMemoryCache`, `AddRateLimiter`, `UseRateLimiter()`
- `src/API/Controllers/AuthController.cs` — `[EnableRateLimiting("login")]`, switch no novo resultado
- `src/Core/Application/Commands/Auth/Login/LoginCommand.cs` — `IRequest<LoginResultado>`, enum/classe de resultado
- `src/Core/Application/Commands/Auth/Login/LoginHandler.cs` — lógica de contagem/bloqueio via `IMemoryCache`

## Riscos e decisões

- **Decisão:** os dois limites ficam com valores fixos no código
  (10/60s por IP; 5 falhas/15min por conta, bloqueio de 15min) — não
  viraram configuração em `appsettings.json`. São parâmetros de
  política de segurança, não algo que varia por ambiente hoje;
  YAGNI até haver necessidade real de ajustar por ambiente.
- **Risco aceito:** `IMemoryCache` é por processo — reiniciar a API
  zera todos os contadores e bloqueios. Documentado como fora de
  escopo (spec, seção "Fora de escopo"); resolver exigiria um store
  compartilhado (Redis, ou tabela no Postgres), o que é
  desproporcional pro estágio atual (single-instance, dev/local).
- **Decisão (evita novo canal de enumeração):** o contador de
  tentativas falhas é incrementado pelo e-mail informado *antes* de
  checar se o usuário existe (dentro do mesmo bloco
  `usuario is null || !Verify(...)`), então um e-mail que não existe
  também acumula tentativas e é bloqueado com `429` depois de 5 —
  igual a um e-mail real. Isso evita que o bloqueio por conta vire
  mais um sinal de "esse e-mail existe" além do canal lateral de
  tempo já documentado em outra spec.
- **Decisão:** `QueueLimit: 0` no limitador por IP — prefere rejeitar
  na hora (`429` imediato) a enfileirar requisições de login, que não
  faz sentido pra esse endpoint.

## Estratégia de verificação

Com a API local rodando:

- 11 requisições seguidas do mesmo IP em menos de 60s → as 10
  primeiras processam normalmente (200/401 conforme credencial), a
  11ª retorna `429`.
- 5 tentativas com senha errada pra uma mesma conta → a 6ª (mesmo com
  IP diferente, simulado trocando o header não se aplica — testar
  program running em loopback mesmo IP) retorna `429`, mesmo se a
  senha estiver certa dessa vez.
- Login correto antes de bater 5 falhas → contador zera, tentativas
  seguintes (erradas) recomeçam a contagem do zero.
- `dotnet build` sem erros.
