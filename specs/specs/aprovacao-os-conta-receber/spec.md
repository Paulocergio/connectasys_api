# Spec — Conclusão de OS gera Conta a Receber

> **Nota:** a pasta manteve o nome original (`aprovacao-os-conta-receber`),
> mas o gatilho mudou depois da primeira rodada de implementação: não é
> mais uma ação dedicada de "aprovar" — é a **mudança de status da OS
> para "Concluído"**, feita pelo fluxo normal de edição. Ver histórico
> no `tasks.md`.

## Objetivo

Hoje, quando uma OS termina, quem faz o lançamento em Contas a Receber
é a pessoa da oficina, copiando o valor da OS na mão. Essa feature liga
as duas pontas: quando o status da OS vira "Concluído", o sistema já
gera automaticamente a conta a receber correspondente, com o valor
certo e o vínculo com a OS de origem — sem passo manual extra.

## User stories

- Como financeiro, quero que toda OS que vira "Concluído" gere
  automaticamente uma conta a receber pendente, sem precisar lançar
  manualmente e sem risco de esquecer ou errar o valor.
- Como financeiro, ao ver uma conta a receber, quero saber se ela veio
  de uma OS (e de qual) pra não confundir com uma conta lançada à mão.
- Como atendente, se eu excluir uma OS por engano (ou porque foi
  cadastrada errada), quero que a conta a receber gerada por ela suma
  junto — não quero uma conta "órfã" sobrando no financeiro.

## Campos novos

### Conta a Receber — campo novo

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| OrdemServicoId | referência à Ordem de Serviço | não | `null` para contas lançadas manualmente (comportamento atual, preservado); preenchido quando a conta nasceu da conclusão de uma OS |

Nenhuma mudança nos campos de `OrdemServico` — `AprovacaoClienteEm`/
`AprovacaoClienteNome` continuam existindo exatamente como antes desta
feature (campos manuais, editáveis pelo `PUT` genérico), sem nenhuma
relação com a geração da conta a receber.

## Critérios de aceite

- Quando uma atualização de OS (`PUT /api/OrdensServico/{id}`) muda o
  `Status` de qualquer outro valor **para** `"Concluído"` (transição,
  não só "já estar concluída"):
  - Se ainda não existe nenhuma conta a receber vinculada a essa OS
    (`OrdemServicoId`), e o valor total da OS é maior que zero, uma
    `ContaReceber` é criada automaticamente com:
    - `ClienteId` = o mesmo `ClienteId` da OS
    - `Descricao` = referência legível à OS incluindo o problema
      relatado, ex.: `"OS #42 — Troca de óleo e filtros"`
    - `Valor` = `ValorMaoDeObra + soma(itens.Quantidade × itens.ValorUnitario) − Desconto`
      da OS no momento da conclusão (mesma fórmula de
      `specs/ordens-servico/spec.md`)
    - `DataVencimento` = data de conclusão da OS + 30 dias corridos
    - `OrdemServicoId` = o id da OS concluída
    - Sem `DataRecebimento` — nasce pendente
  - A atualização da OS e a criação da conta a receber acontecem como
    uma operação só: se uma falhar, a outra não fica gravada sozinha.
- Salvar a OS de novo com `Status = "Concluído"` (já estava concluída
  antes dessa atualização) **não** gera uma segunda conta — a
  transição só conta uma vez, verificada pela ausência de uma conta já
  vinculada àquela OS.
- OS com valor total igual a zero pode ser marcada como concluída
  normalmente, mas **não gera** conta a receber (sem lançamento de
  valor zero no financeiro).
- **Excluir uma OS remove junto qualquer conta a receber gerada a
  partir dela** (não fica órfã, não vira `OrdemServicoId = null`) —
  diferente de uma exclusão de cliente/usuário em outras partes do
  sistema, aqui a conta em si deixa de existir.
- `GET` de conta a receber (lista e por id) passa a retornar também
  `OrdemServicoId` (pode ser `null`).
- Contas a receber lançadas manualmente (sem OS de origem) continuam
  funcionando exatamente como hoje — `OrdemServicoId = null`, sem
  nenhuma restrição nova de edição/remoção.

- **Se a OS for cancelada (`Status = "Cancelado"`) e já tinha uma conta
  a receber vinculada, essa conta é excluída** — uma OS cancelada não
  pode deixar cobrança pendente no financeiro. Diferente da exclusão
  da própria OS (que também exclui a conta via `ON DELETE CASCADE`),
  aqui a OS continua existindo, só a conta é removida.
- Se a OS já tem uma conta a receber vinculada (`OrdemServicoId`) e o
  valor total muda depois — por edição de `ValorMaoDeObra`/`Desconto`
  (`PUT`) ou por adicionar/remover item — o `Valor` dessa conta é
  **atualizado automaticamente** pra refletir o novo total. Vale tanto
  pra `PUT /api/OrdensServico/{id}` quanto pra
  `POST/DELETE .../itens`.

## Fora de escopo (por enquanto)

- Voltar o status de "Concluído" pra outro valor não reverte nem
  remove a conta a receber já gerada (só a exclusão da OS remove).
- Editar ou remover uma `ContaReceber` que tem `OrdemServicoId`
  preenchido — continua permitido sem nenhuma trava especial.
- Gerar conta a pagar (ex.: peças compradas de fornecedor) a partir da
  OS — só o lado de Contas a Receber está coberto aqui.

## Suposições e Perguntas em Aberto

- Decidido: o gatilho é a transição de status para "Concluído" pelo
  fluxo normal de edição de OS, não uma ação separada de aprovação.
- Decidido: OS com valor total zero pode ser concluída normalmente,
  mas não gera conta a receber.
- Decidido: excluir a OS exclui a conta a receber vinculada (FK com
  `ON DELETE CASCADE`, não `SET NULL`).
- Decidido: a descrição da conta gerada inclui o problema relatado da
  OS (`DescricaoProblema`), não só o número.
