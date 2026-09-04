# Spec — Perfis de Usuário (Roles fixas)

## Objetivo

Hoje `Usuario.Role` é texto livre (o form do hub sugere "Admin, Manager,
User..." só como placeholder, sem nenhuma validação real). A oficina
precisa de papéis fixos e conhecidos, refletindo os cargos reais da
equipe, pra padronizar quem pode ser cadastrado e permitir telas/regras
futuras baseadas em papel.

## User stories

- Como administrador, ao cadastrar ou editar um usuário, quero escolher
  o papel dele entre um conjunto fixo de opções, não digitar texto livre.
- Como administrador, quero que a API rejeite um papel fora desse
  conjunto, tanto ao criar quanto ao editar.

## Campos

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Role | texto (conjunto fechado) | sim | um de: `Admin`, `Mecânico`, `Recepcionista`, `Financeiro` |

## Critérios de aceite

- Criar usuário com `Role` fora do conjunto → `400`.
- Atualizar usuário com `Role` fora do conjunto → `400`.
- Criar/atualizar com um dos 4 valores válidos → comportamento atual
  preservado (`201`/`204`), sem mudança nos critérios já existentes
  (e-mail duplicado continua `409`, id inexistente continua `404`).
- Listar/buscar usuário continua retornando `Role` como veio (string).

## Fora de escopo (por enquanto)

- Múltiplos papéis por usuário.
- Permissões/autorização por papel (`[Authorize(Roles=...)]`) — o
  projeto ainda não protege nenhum endpoint (constitution, seção 6).
- Papéis customizáveis/cadastráveis pela oficina — as 4 opções são
  fixas no código, mesmo padrão de `StatusConta`/`FormasPagamento`.
