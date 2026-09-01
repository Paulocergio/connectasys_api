# Spec — Contas a Pagar

## Objetivo

Permitir que a oficina registre e controle as contas que tem a pagar (fornecedores, aluguel, contas de consumo etc.), sabendo quais estão pendentes, pagas ou atrasadas.

## User stories

- Como atendente/administrador, quero cadastrar uma conta a pagar, com valor, vencimento e fornecedor/descrição.
- Como atendente, quero listar todas as contas a pagar.
- Como atendente, quero buscar uma conta a pagar específica pelo id.
- Como atendente, quero atualizar os dados de uma conta a pagar, incluindo marcar a data em que foi paga.
- Como atendente, quero remover uma conta a pagar.
- Como atendente, quero ver, em cada conta retornada pela API, se ela está paga, pendente ou atrasada, sem precisar calcular isso manualmente a partir das datas.

## Campos

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Descricao | texto | sim | ex: "Aluguel setembro/2026" |
| Fornecedor | texto | sim | texto livre — não há entidade `Fornecedor` cadastrada (fora de escopo) |
| Valor | decimal | sim | valor da conta |
| DataVencimento | data | sim | |
| DataPagamento | data | não | preenchida quando a conta é paga; `null` enquanto pendente |
| DataCadastro | data/hora | não (automático) | preenchido no momento da criação |

`Status` **não é um campo persistido** — é derivado e exposto só no DTO de saída (ver `design.md`): "Paga" (tem `DataPagamento`), "Atrasada" (sem `DataPagamento` e `DataVencimento` já passou), "Pendente" (sem `DataPagamento` e `DataVencimento` ainda não passou).

## Critérios de aceite

- Criar conta retorna `201 Created` com a conta criada (incluindo `Id` gerado e `Status` calculado).
- `DataPagamento` não é aceita na criação — só existe via atualização (uma conta nasce sempre pendente).
- Atualizar aceita `DataPagamento` opcionalmente; se enviada, marca a conta como paga; se omitida/nula, a conta permanece sem data de pagamento (pendente ou atrasada, conforme vencimento).
- Buscar conta por id inexistente retorna `404 Not Found`.
- Atualizar conta com id inexistente retorna `404 Not Found`.
- Remover conta com id inexistente retorna `404 Not Found`.
- Atualizar ou remover com sucesso retorna `204 No Content`.
- Listar contas retorna `200 OK` com array (vazio se não houver nenhuma), cada item com `Status` calculado.

## Fora de escopo (por enquanto)

- Entidade `Fornecedor` cadastrável (fica como texto livre).
- Validação de valor (não pode ser negativo, etc. — ver constitution, seção 6, sem FluentValidation ainda).
- Autenticação/autorização no endpoint.
- Pagamento parcial / múltiplos pagamentos para a mesma conta.
- Recorrência (contas fixas mensais geradas automaticamente).
- Anexo de comprovante de pagamento.
- Vínculo com ordens de serviço.
