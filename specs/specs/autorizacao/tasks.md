# Tasks — Autorização (proteger os endpoints)

- [x] Adicionar `[Authorize(Roles = Roles.Admin)]` em `UsuariosController`
- [x] Adicionar `[Authorize]` em `ClientesController`
- [x] Adicionar `[Authorize]` em `VeiculosController`
- [x] Adicionar `[Authorize]` em `ContasPagarController`
- [x] Adicionar `[Authorize]` em `ContasReceberController`
- [x] `dotnet build` sem erros
- [x] Testar via `curl`:
  - [x] Sem token: `GET /api/Clientes` e `GET /api/Usuarios` → `401` nos dois
  - [x] Login com usuário Admin de teste → token ok
  - [x] Token `Admin`: `GET /api/Usuarios` → `200`; criar usuário → `201`
  - [x] Login com usuário `Financeiro` de teste → token ok
  - [x] Token `Financeiro`: `GET /api/Clientes` → `200`; `GET
        /api/Usuarios` → `403`; tentar criar usuário `Admin` → `403`
        (confirma que a escalação de privilégio da Vuln 2 foi fechada)
  - [x] Token adulterado (string extra no fim) → `401`
  - [x] `POST /api/Auth/login` continua funcionando sem token
  - [x] Usuários Admin e Financeiro de teste removidos depois

**Status: concluído.** Testado ponta a ponta via API real. Nota
importante: **não existe usuário Admin real no banco agora** — o único
Admin criado durante o teste foi removido no final (era só de teste).
O dono do projeto precisa criar um Admin de verdade antes de usar o
sistema, senão ninguém consegue mais gerenciar usuários.
