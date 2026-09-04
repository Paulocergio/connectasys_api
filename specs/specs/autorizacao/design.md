# Design — Autorização (proteger os endpoints)

## Abordagem

`[Authorize]` do ASP.NET Core, usando o esquema JWT Bearer já
configurado em `Program.cs` (`AddAuthentication().AddJwtBearer(...)`)
— nenhuma peça nova de infraestrutura, só aplicar o atributo.

| Controller | Atributo | Motivo |
|---|---|---|
| `AuthController` | nenhum (fica público) | precisa ser acessível sem token pra permitir o login |
| `UsuariosController` | `[Authorize(Roles = Roles.Admin)]` na classe | gestão de equipe/contas é só de Admin — fecha a escalação de privilégio (qualquer um podia se auto-cadastrar como Admin) |
| `ClientesController` | `[Authorize]` na classe | qualquer role autenticada usa esse recurso no dia a dia |
| `VeiculosController` | `[Authorize]` na classe | idem |
| `ContasPagarController` | `[Authorize]` na classe | idem |
| `ContasReceberController` | `[Authorize]` na classe | idem |

`[Authorize(Roles = Roles.Admin)]` funciona porque `TokenService`
já grava a role em `ClaimTypes.Role` (`TokenService.cs:28`) — que é
exatamente o claim type que o middleware de autorização usa por
padrão pra resolver `User.IsInRole(...)`/`Roles = "..."`. Não precisa
mexer no `TokenService`.

## Arquivos alterados

- `src/API/Controllers/UsuariosController.cs` — `[Authorize(Roles = Roles.Admin)]`
- `src/API/Controllers/ClientesController.cs` — `[Authorize]`
- `src/API/Controllers/VeiculosController.cs` — `[Authorize]`
- `src/API/Controllers/ContasPagarController.cs` — `[Authorize]`
- `src/API/Controllers/ContasReceberController.cs` — `[Authorize]`
- `src/API/Controllers/AuthController.cs` — sem alteração

## Riscos e decisões

- **Decisão:** aplicar o atributo na classe do controller (não em cada
  action individualmente) — mais simples, e nenhum endpoint desses
  controllers deveria ficar público.
- **Risco:** `connectasys-hub` (frontend) já manda `Authorization:
  Bearer <token>` em todo `apiFetch` quando há sessão (`src/lib/api.ts`),
  então o hub não deveria quebrar — mas usuários sem sessão ativa no
  hub (ou chamadas feitas fora do fluxo de login) passam a receber
  `401` em vez de dado real. Isso é o comportamento esperado da spec,
  não um bug.
- **Risco:** não existe hoje nenhum usuário `Admin` no banco de dados
  local — só `Financeiro` e `Recepcionista`. Sem um Admin, ninguém
  consegue mais gerenciar usuários depois dessa mudança. Precisa criar
  (ou promover) um usuário Admin antes/durante a verificação.

## Estratégia de verificação

Com a API local rodando:

- Sem token: `GET /api/Clientes`, `GET /api/Usuarios` → `401` nos dois.
- Login com usuário `Financeiro` existente → token válido.
- Com token de `Financeiro`: `GET /api/Clientes` → `200`; `GET
  /api/Usuarios` → `403`.
- Criar um usuário `Admin` de teste (via banco, já que o endpoint some
  em seguida) e logar com ele → token válido.
- Com token de `Admin`: `GET /api/Usuarios` → `200`; `POST
  /api/Usuarios` → `201`.
- Token expirado/assinatura inválida (token adulterado manualmente) →
  `401`.
- `POST /api/Auth/login` sem token → continua `200`/`401` conforme
  credencial, nunca bloqueado por autenticação.
