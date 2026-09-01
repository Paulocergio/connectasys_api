# Spec — Veículos

## Objetivo

Permitir que a oficina cadastre e gerencie os veículos dos clientes, vinculando cada veículo a um cliente já cadastrado — base para uma futura feature de ordens de serviço, que será vinculada a um veículo.

## User stories

- Como atendente da oficina, quero cadastrar um veículo novo, vinculado a um cliente existente.
- Como atendente, quero listar todos os veículos cadastrados.
- Como atendente, quero listar os veículos de um cliente específico.
- Como atendente, quero buscar um veículo específico pelo id.
- Como atendente, quero atualizar os dados de um veículo.
- Como atendente, quero remover um veículo.

## Campos

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| ClienteId | inteiro (FK) | sim | referencia um `Cliente` já cadastrado |
| Placa | texto | sim | |
| Marca | texto | sim | |
| Modelo | texto | sim | |
| Ano | inteiro | sim | ano do veículo (um único campo, sem separar fabricação/modelo) |
| Cor | texto | sim | |
| DataCadastro | data/hora | não (automático) | preenchido no momento da criação |

## Critérios de aceite

- Criar veículo retorna `201 Created` com o veículo criado (incluindo o `Id` gerado).
- Criar veículo com `ClienteId` de um cliente inexistente retorna `400 Bad Request`.
- Buscar veículo por id inexistente retorna `404 Not Found`.
- Atualizar veículo com id inexistente retorna `404 Not Found`.
- Atualizar veículo com `ClienteId` de um cliente inexistente retorna `400 Bad Request`.
- Remover veículo com id inexistente retorna `404 Not Found`.
- Atualizar ou remover com sucesso retorna `204 No Content`.
- Listar todos os veículos retorna `200 OK` com array (vazio se não houver nenhum).
- Listar veículos de um cliente retorna `200 OK` com array (vazio se o cliente não tiver veículos); se o `clienteId` da rota não existir, retorna `200 OK` com array vazio (mesmo comportamento de "sem veículos", sem checagem extra de existência do cliente nessa rota de listagem).

## Fora de escopo (por enquanto)

- Validação de formato/unicidade de placa (ver constitution, seção 6 — sem FluentValidation ainda).
- Autenticação/autorização no endpoint.
- Vínculo com ordens de serviço (feature futura).
- Histórico de troca de dono do veículo (atualizar `ClienteId` simplesmente reatribui o veículo a outro cliente, sem registro do dono anterior).
