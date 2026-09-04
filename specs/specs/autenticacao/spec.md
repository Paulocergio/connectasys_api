# Spec — Autenticação (Login)

## Objetivo

Permitir que um usuário já cadastrado (ver spec [usuarios](../usuarios/spec.md)) se autentique com email e senha e receba um token de acesso (JWT), para uso futuro em endpoints protegidos por `[Authorize]`.

## User stories

- Como usuário cadastrado, quero fazer login com meu email e senha para obter um token de acesso.
- Como usuário, quero receber uma mensagem de erro genérica (sem indicar se o problema foi o email ou a senha) quando as credenciais estiverem incorretas.

## Campos

### Requisição (login)

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Email | texto | sim | |
| Senha | texto | sim | texto puro, comparada contra o hash via `IPasswordHasher.Verify` |

### Resposta (sucesso)

| Campo | Tipo | Observação |
|---|---|---|
| Token | texto | JWT assinado |
| ExpiraEmUtc | data/hora | momento de expiração do token |
| UsuarioId | Guid | |
| Nome | texto | |
| Role | texto | incluído como claim no token e no corpo da resposta, para a UI consumir sem decodificar o JWT |

## Critérios de aceite

- Login com email e senha corretos retorna `200 OK` com `Token`, `ExpiraEmUtc`, `UsuarioId`, `Nome` e `Role`.
- Login com email inexistente retorna `401 Unauthorized` com mensagem genérica ("Email ou senha inválidos").
- Login com senha incorreta retorna `401 Unauthorized` com a mesma mensagem genérica (não deve ser possível diferenciar "email não existe" de "senha errada" pela resposta).
- O token gerado contém, no mínimo, as claims: id do usuário (`sub`), email, nome e role.
- A senha em texto puro nunca é logada, persistida ou incluída em qualquer resposta.

## Fora de escopo (por enquanto)

- Proteger os endpoints existentes (`Clientes`, `Usuarios`, `Veiculos`, `ContasPagar`, `ContasReceber`) com `[Authorize]` — esta spec só cobre a emissão do token. Aplicar `[Authorize]` nos controllers existentes é uma mudança maior (afeta todos eles) e fica para uma spec futura.
- Refresh token.
- Recuperação/reset de senha ("esqueci minha senha").
- Multi-tenant (`empresa_id`) — o token não carrega claim de empresa ainda, pois a entidade `Usuario` também não tem esse campo hoje.
- Rate limiting / bloqueio por tentativas de login repetidas.
