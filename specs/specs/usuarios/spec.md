# Spec — Usuários

## Objetivo

Gerenciar a equipe interna da oficina (mecânicos, atendentes, administradores) que vai operar o sistema — base para uma futura camada de autenticação e permissões por papel (`role`).

## User stories

- Como administrador da oficina, quero cadastrar um novo usuário da equipe, definindo seu papel (`role`) e uma senha de acesso.
- Como administrador, quero listar todos os usuários cadastrados, sem que a senha ou seu hash apareçam em nenhuma informação retornada.
- Como administrador, quero buscar um usuário específico pelo id.
- Como administrador, quero atualizar os dados de um usuário, podendo opcionalmente trocar sua senha.
- Como administrador, quero remover um usuário.

## Campos (versão atual, simplificada)

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Nome | texto | sim | |
| Email | texto | sim | |
| Senha | texto (somente entrada) | sim na criação / opcional na atualização | Recebida em texto puro na requisição; nunca é persistida em texto puro nem devolvida pela API — só o hash é armazenado. Se omitida na atualização, a senha atual permanece inalterada. |
| Role | texto | sim | ex: "admin", "mecanico", "atendente" — sem lista fixa ainda |
| Telefone | texto | sim | |
| DataCriacaoUtc | data/hora | não (automático) | preenchido no momento da criação |

## Critérios de aceite

- **O e-mail é único entre os usuários.** Criar um usuário com um e-mail já
  cadastrado (comparação exata, sensível a maiúsculas/minúsculas — mesma
  regra já usada na busca de login) não cria um segundo registro: retorna
  `409 Conflict` com uma mensagem clara. Atualizar um usuário para um
  e-mail que já pertence a **outro** usuário tem o mesmo comportamento;
  atualizar um usuário mantendo o próprio e-mail continua funcionando
  normalmente.
- Criar usuário retorna `201 Created` com o usuário criado (incluindo o `Id` gerado como `Guid`).
- Criar usuário exige uma senha em texto puro no corpo da requisição; a senha nunca é gravada em texto puro no banco — apenas o hash é persistido (coluna `senha_hash`).
- Atualizar usuário aceita opcionalmente uma nova senha; se enviada, o hash é recalculado e substitui o hash anterior; se omitida, a senha atual não é alterada.
- **A senha e o hash da senha nunca aparecem em nenhuma resposta da API** — nem na criação, nem na busca por id, nem na listagem, nem na atualização. Nenhum DTO de saída expõe esses campos.
- Buscar usuário por id inexistente retorna `404 Not Found`.
- Atualizar usuário com id inexistente retorna `404 Not Found`.
- Remover usuário com id inexistente retorna `404 Not Found`.
- Atualizar ou remover com sucesso retorna `204 No Content`.
- Listar usuários retorna `200 OK` com array (vazio se não houver nenhum).

## Fora de escopo (por enquanto)

- **Autenticação (login, JWT, refresh token)** — esta spec cobre apenas o hash de senha no cadastro e na atualização de usuário. Login/autenticação é feature separada e futura, com spec própria.
- Recuperação/reset de senha ("esqueci minha senha").
- Validação de formato de email/telefone ou de força/complexidade da senha (ex: tamanho mínimo, caracteres especiais).
- Restrição de valores possíveis para `Role` (enum/lista fixa).
- Multi-tenant (`empresa_id`).
- Normalização de e-mail (ex.: tratar `Joao@x.com` e `joao@x.com` como o
  mesmo e-mail) — a checagem de duplicidade usa comparação exata, igual à
  já existente na busca de login.
