# Spec — Contas a Receber

## Objetivo

Permitir que a oficina registre e controle os valores que tem a receber de clientes, sabendo quais estão pendentes, recebidos ou atrasados.

## User stories

- Como atendente/administrador, quero cadastrar uma conta a receber, vinculada a um cliente já cadastrado, com valor e vencimento.
- Como atendente, quero listar todas as contas a receber.
- Como atendente, quero listar as contas a receber de um cliente específico.
- Como atendente, quero buscar uma conta a receber específica pelo id.
- Como atendente, quero atualizar os dados de uma conta a receber, incluindo marcar a data em que foi recebida.
- Como atendente, quero remover uma conta a receber.
- Como atendente, quero ver, em cada conta retornada pela API, se ela está recebida, pendente ou atrasada, sem precisar calcular isso manualmente a partir das datas.

## Campos

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| ClienteId | inteiro (FK) | sim | referencia um `Cliente` já cadastrado |
| Descricao | texto | sim | ex: "Troca de óleo + revisão" |
| Valor | decimal | sim | valor a receber |
| DataVencimento | data | sim | |
| DataRecebimento | data | não | preenchida quando o valor é recebido; `null` enquanto pendente |
| DataCadastro | data/hora | não (automático) | preenchido no momento da criação |

`Status` **não é um campo persistido** — mesmo cálculo derivado de `contas-pagar/spec.md`, adaptado: "Recebida" (tem `DataRecebimento`), "Atrasada" (sem `DataRecebimento` e `DataVencimento` já passou), "Pendente" (sem `DataRecebimento` e `DataVencimento` ainda não passou).

## Critérios de aceite

- Criar conta retorna `201 Created` com a conta criada (incluindo `Id` gerado e `Status` calculado).
- Criar conta com `ClienteId` de um cliente inexistente retorna `400 Bad Request`.
- `DataRecebimento` não é aceita na criação — só existe via atualização (uma conta nasce sempre pendente).
- Atualizar aceita `DataRecebimento` opcionalmente; se enviada, marca a conta como recebida; se omitida/nula, a conta permanece sem data de recebimento (pendente ou atrasada, conforme vencimento).
- Atualizar com `ClienteId` de um cliente inexistente retorna `400 Bad Request`.
- Buscar conta por id inexistente retorna `404 Not Found`.
- Atualizar conta com id inexistente retorna `404 Not Found`.
- Remover conta com id inexistente retorna `404 Not Found`.
- Atualizar ou remover com sucesso retorna `204 No Content`.
- Listar contas retorna `200 OK` com array (vazio se não houver nenhuma), cada item com `Status` calculado.
- Listar contas de um cliente retorna `200 OK` com array (vazio se o cliente não tiver contas ou não existir — mesmo comportamento já adotado em `veiculos/spec.md`).

## Fora de escopo (por enquanto)

- Validação de valor (não pode ser negativo, etc. — ver constitution, seção 6, sem FluentValidation ainda).
- Autenticação/autorização no endpoint.
- Recebimento parcial / múltiplos recebimentos para a mesma conta.
- Recorrência.
- Vínculo com ordens de serviço (contas a receber ainda não nascem automaticamente de uma OS — feature futura).
