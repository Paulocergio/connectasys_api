# Spec — Forma de Pagamento (Contas a Pagar e Contas a Receber)

## Objetivo

Permitir registrar **como** uma conta a pagar foi paga ou uma conta a
receber foi recebida — Cartão, Pix, Boleto ou Dinheiro — além de
apenas a data. Hoje `ContaPagar`/`ContaReceber` só guardam
`DataPagamento`/`DataRecebimento`; não há como saber a forma usada.
Afeta as duas entidades da mesma forma, então é tratada como uma única
feature.

## User stories

- Como atendente/administrador, ao marcar uma conta a pagar como paga
  (informando `DataPagamento`), quero registrar também a forma de
  pagamento usada.
- Como atendente/administrador, ao marcar uma conta a receber como
  recebida (informando `DataRecebimento`), quero registrar também a
  forma de recebimento usada.
- Como atendente, quero ver a forma de pagamento/recebimento ao
  consultar uma conta já paga/recebida.

## Campos

| Campo | Entidade | Tipo | Obrigatório | Observação |
|---|---|---|---|---|
| FormaPagamento | `ContaPagar` | texto (conjunto fechado) | condicional | um de: `Cartão`, `Pix`, `Boleto`, `Dinheiro`; `null` enquanto a conta não tem `DataPagamento` |
| FormaPagamento | `ContaReceber` | texto (conjunto fechado) | condicional | mesmo conjunto; `null` enquanto a conta não tem `DataRecebimento` |

Nome do campo é `FormaPagamento` nas duas entidades (não
`FormaRecebimento` em `ContaReceber`) — mantém o termo único do
domínio, já que é sempre "como o dinheiro se moveu", só que em
direções diferentes.

## Critérios de aceite

- Atualizar uma conta informando `DataPagamento`/`DataRecebimento`
  **sem** informar `FormaPagamento` é rejeitado (`400`) — a forma é
  obrigatória sempre que a conta está sendo marcada como paga/recebida
  nessa chamada.
- Atualizar uma conta **sem** informar `DataPagamento`/
  `DataRecebimento` (deixando `null`) ignora `FormaPagamento` mesmo que
  enviado — não faz sentido ter forma de pagamento numa conta ainda não
  paga.
- `FormaPagamento` só aceita um dos 4 valores do conjunto fechado; um
  valor fora da lista é rejeitado (`400`).
- Criar conta (`POST`) continua sem aceitar `DataPagamento`/
  `DataRecebimento` nem `FormaPagamento` — toda conta nasce pendente,
  sem mudança nesse comportamento já existente.
- Consultar uma conta (`GET`, individual ou listagem) retorna
  `FormaPagamento` (`null` se a conta ainda não foi paga/recebida).

## Fora de escopo (por enquanto)

- Conjunto de formas de pagamento configurável/cadastrável — as 4
  opções são fixas no código, igual ao padrão já usado em `StatusConta`.
- Pagamento parcial ou combinação de mais de uma forma na mesma conta.
- Dados adicionais por forma (ex: últimos 4 dígitos do cartão,
  identificador do Pix, número do boleto) — só a forma em si.
- Validação de valor/negativo (fora de escopo do projeto como um todo,
  ver constitution seção 6).
