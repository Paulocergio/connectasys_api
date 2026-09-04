# Spec — Autorização (proteger os endpoints)

## Objetivo

Hoje a API emite um JWT válido no login (`POST /api/Auth/login`), mas
nenhum endpoint exige esse token — qualquer pessoa, sem credencial
nenhuma, lê e altera todos os dados (clientes, veículos, contas,
usuários). Esta spec fecha essa brecha: só quem tem um token válido
acessa a API, e só um Admin gerencia usuários (evita que qualquer um
se auto-cadastre como Admin).

## User stories

- Como dono da oficina, quero que ninguém sem login consiga ver ou
  alterar dados da minha oficina pela API.
- Como Admin, quero ser o único que pode criar, editar ou remover
  contas de usuário (equipe da oficina) — as outras roles não devem
  conseguir se promover ou gerenciar colegas.
- Como membro da equipe autenticado (qualquer role), quero continuar
  acessando normalmente clientes, veículos e contas a pagar/receber.

## Critérios de aceite

- Chamar qualquer endpoint (exceto `POST /api/Auth/login`) sem header
  `Authorization` retorna `401 Unauthorized`.
- Chamar qualquer endpoint com um token expirado ou com assinatura
  inválida retorna `401 Unauthorized`.
- Chamar `GET/POST/PUT/DELETE /api/Usuarios*` com um token válido cuja
  role não é `Admin` retorna `403 Forbidden`.
- Chamar `GET/POST/PUT/DELETE /api/Usuarios*` com um token válido de
  role `Admin` funciona normalmente (mesmo comportamento de hoje).
- Chamar os endpoints de Clientes, Veículos, ContasPagar e
  ContasReceber com um token válido de **qualquer** role funciona
  normalmente (mesmo comportamento de hoje) — a restrição por role é
  só em Usuários.
- `POST /api/Auth/login` continua acessível sem token (senão ninguém
  conseguiria logar).

## Fora de escopo (por enquanto)

- Autorização granular por role nos outros recursos (ex.: só
  Financeiro mexe em ContasPagar) — não foi pedido; hoje qualquer
  usuário logado acessa todos os módulos exceto Usuários.
- Refresh token / renovação de sessão.
- Alterar o formato ou os claims do JWT (já usa `ClaimTypes.Role`, que
  é o que `[Authorize(Roles = ...)]` espera — nenhuma mudança
  necessária em `TokenService`).
- Multi-tenant (`empresa_id`) — fora do estado atual do projeto (ver
  constitution, seção 6).
