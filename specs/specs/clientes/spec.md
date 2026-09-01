# Spec — Clientes

## Objetivo

Permitir que a oficina cadastre e gerencie os clientes dela, para que futuramente ordens de serviço e veículos possam ser vinculados a um cliente.

## User stories

- Como atendente da oficina, quero cadastrar um cliente novo.
- Como atendente, quero listar todos os clientes cadastrados.
- Como atendente, quero buscar um cliente específico pelo id.
- Como atendente, quero atualizar os dados de um cliente.
- Como atendente, quero remover um cliente.

## Campos

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Nome | texto | sim | |
| Email | texto | sim | |
| Telefone | texto | sim | |
| DataCadastro | data/hora | não (automático) | preenchido no momento da criação |

## Critérios de aceite

- Criar cliente retorna `201 Created` com o cliente criado (incluindo o `Id` gerado).
- Buscar cliente por id inexistente retorna `404 Not Found`.
- Atualizar cliente com id inexistente retorna `404 Not Found`.
- Remover cliente com id inexistente retorna `404 Not Found`.
- Atualizar ou remover com sucesso retorna `204 No Content`.
- Listar clientes retorna `200 OK` com array (vazio se não houver nenhum).

## Fora de escopo (por enquanto)

- Validação de formato de email/telefone (ver constitution, seção 6).
- Autenticação/autorização no endpoint.
- Vínculo com veículos ou ordens de serviço (features futuras).
