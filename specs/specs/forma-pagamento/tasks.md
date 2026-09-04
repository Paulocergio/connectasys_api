# Tasks — Forma de Pagamento (Contas a Pagar e Contas a Receber)

- [x] Criar `FormasPagamento` (`Core/Application/Common/FormasPagamento.cs`)
- [x] Adicionar `FormaPagamento` (`string?`) em `ContaPagar` e `ContaReceber`
- [x] Mapear `forma_pagamento` em `ContaPagarConfiguration` e `ContaReceberConfiguration`
- [x] Adicionar `FormaPagamento` em `ContaPagarDto` e `ContaReceberDto` (e nos 7 handlers que os montam: 2 `Create*`, 5 `Get*`)
- [x] `UpdateContaPagarCommand`: adicionar `FormaPagamento`; mudar retorno de `bool` para `UpdateContaPagarResult` (Success/ContaNotFound/FormaPagamentoInvalida)
- [x] `UpdateContaPagarHandler`: aplicar a regra de validação (FormaPagamento obrigatória e válida quando DataPagamento é informado; ignorada/`null` quando não é)
- [x] `ContasPagarController.Update`: trocar `bool`/`NotFound` simples pelo switch de `UpdateContaPagarResult`
- [x] `UpdateContaReceberCommand`/`UpdateContaReceberResult`: adicionar `FormaPagamento` e o caso `FormaPagamentoInvalida`
- [x] `UpdateContaReceberHandler`: aplicar a mesma regra (com `DataRecebimento`)
- [x] `ContasReceberController.Update`: adicionar o novo `case` no switch existente
- [x] Gerar migration (`20260903215250_AddFormaPagamentoContasPagarEContasReceber`)
- [x] Aplicar migration localmente (`dotnet ef database update`)
- [x] `dotnet build` sem erros
- [x] Testar via `curl` (ContasPagar e ContasReceber):
  - [x] Atualizar com `dataPagamento`/`dataRecebimento` + `formaPagamento` válida → `204`; `GET` confirma `status`="Paga" e `formaPagamento`
  - [x] Atualizar com `dataPagamento`/`dataRecebimento` sem `formaPagamento` → `400`
  - [x] Atualizar com `formaPagamento` fora do conjunto (ex: "Cheque") → `400`
  - [x] Atualizar sem `dataPagamento`/`dataRecebimento` → `204`, `formaPagamento` gravado como `null`
  - [x] Registro de teste removido após os testes

**Status: concluído.** Testado ponta a ponta via API real (contas de teste
criadas e removidas após os testes). Nota de execução: o primeiro teste
de `ContasReceber` com `formaPagamento: "Cartão"` deu `400` por um
problema de encoding UTF-8 do `curl` inline no shell local (não um bug
da API) — reconfirmado com o payload escrito em arquivo, resultado
`204`/`"Cartão"` corretos.
