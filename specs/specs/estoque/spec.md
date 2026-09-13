# Spec — Estoque

> **Revisão 2026-09-13** (pedido do usuário): campo `EstoqueMinimo`
> removido por completo (backend, DTO, coluna da tabela) — junto some
> o aviso de estoque baixo, que dependia dele. Sem mudança de campo no
> backend pra Margem de Venda/Margem de Markup: continuam calculadas
> só no frontend a partir de `PrecoCompra`/`PrecoVenda`, que já
> existem — ver `specs/estoque/design.md` do `connectasys-hub`.

## Objetivo

Permitir que a oficina cadastre e controle as peças e materiais que tem
em estoque (descrição, quantidade disponível, valor unitário). Além do
CRUD, o estoque se conecta com Ordens de Serviço: ao adicionar um item
(peça) numa OS a partir do estoque, a descrição e o valor unitário são
preenchidos automaticamente e a quantidade em estoque é debitada — ao
remover o item da OS, a quantidade volta pro estoque.

## User stories

- Como atendente, quero cadastrar uma peça/material no estoque, com
  quantidade disponível e valor unitário.
- Como atendente, quero listar, buscar, atualizar e remover peças do
  estoque.
- Como mecânico, ao adicionar um item numa Ordem de Serviço, quero
  escolher a peça direto do estoque em vez de digitar descrição e
  valor na mão, evitando erro de preço.
- Como dono da oficina, quero que o estoque reflita o uso real das
  peças — quando uma peça é usada numa OS, a quantidade disponível
  diminui; se o item for removido da OS, a quantidade volta.
- Como dono da oficina, quero saber quanto pago e quanto cobro por
  cada peça, e ver a margem de lucro que estou tendo em cima dela.
- Como dono da oficina, quero ser avisado quando uma peça está com
  estoque baixo, pra saber que preciso comprar mais.

## Campos

### Estoque

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Id | inteiro sequencial | sim (automático) | |
| Nome | texto | sim | nome curto da peça/material |
| Descricao | texto | não | detalhe adicional (marca, especificação) |
| Quantidade | decimal | sim | quantidade disponível no estoque |
| PrecoCompra | decimal | sim | quanto a oficina pagou pela peça |
| PrecoVenda | decimal | sim | quanto a oficina cobra do cliente (é o valor usado quando a peça é adicionada numa OS) |
| DataCadastro | data/hora | não (automático) | preenchida na criação |

`EstoqueMinimo` **removido (revisão 2026-09-13)** — não existe mais
como campo, coluna, nem aviso de estoque baixo em nenhuma tela.

`Margem` (percentual de lucro) **não é um campo gravado** — o
frontend passa a calcular duas variantes a partir dos mesmos dois
preços, sempre na hora (evita a margem exibida ficar desatualizada se
um dos preços for editado depois):

- Margem de Markup: `(PrecoVenda − PrecoCompra) / PrecoCompra × 100`
  (era a única "margem" antes desta revisão).
- Margem de Venda: `(PrecoVenda − PrecoCompra) / PrecoVenda × 100`.

Ver `design.md` do frontend (`connectasys-hub`) para a mecânica de
edição bidirecional (editar a Margem de Markup recalcula `PrecoVenda`,
mantendo `PrecoCompra` fixo).

### Item da Ordem de Serviço — campo novo

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| EstoqueId | referência ao Estoque | não | `null` quando o item foi digitado livremente (não veio do estoque); preenchido quando o item foi selecionado de uma peça do estoque |

## Critérios de aceite — CRUD de Estoque

- Criar peça retorna `201 Created`.
- Buscar/atualizar/remover peça com id inexistente retorna `404`.
- Atualizar ou remover com sucesso retorna `204 No Content`.
- Listar todas as peças retorna `200 OK` com array (vazio se não
  houver nenhuma).
- Remover uma peça do estoque **não** afeta itens de OS que já a
  referenciaram no passado (`ItemOrdemServico.EstoqueId` fica
  apontando pra um registro que pode não existir mais — ver Fora de
  Escopo).

## Critérios de aceite — Integração com Ordem de Serviço

- Adicionar um item na OS informando `EstoqueId`:
  - A descrição do item vira o `Nome` da peça, e o valor unitário
    vira o `PrecoVenda` do registro de estoque **no momento da
    adição** (o servidor busca esses dados, não confia no que o
    cliente mandar pra esses dois campos).
  - Se a `Quantidade` do item pedida for maior que a `Quantidade`
    disponível no estoque, a operação é rejeitada com erro claro
    (`400`) — não deixa o estoque ficar negativo.
  - Se a operação for aceita, a `Quantidade` do estoque é debitada no
    valor usado pelo item, como parte da mesma operação (se uma parte
    falhar, a outra não fica gravada sozinha).
- Adicionar um item **sem** `EstoqueId` continua funcionando
  exatamente como hoje — descrição e valor digitados livremente, sem
  nenhum efeito no estoque.
- Remover um item que tem `EstoqueId`:
  - A `Quantidade` debitada na criação desse item volta pro estoque
    (soma de volta), como parte da mesma operação.
  - Se o registro de estoque referenciado não existir mais (foi
    excluído), a remoção do item continua funcionando normalmente,
    só sem devolver quantidade a lugar nenhum.
- Excluir uma Ordem de Serviço inteira: todo item dela que tinha
  `EstoqueId` devolve a quantidade pro estoque correspondente, do
  mesmo jeito que remover os itens individualmente devolveria — a
  exclusão da OS não pode "vazar" quantidade debitada sem devolver.
- `GET` de item de OS (dentro do `GET` de uma OS) passa a retornar
  também `EstoqueId` (pode ser `null`).

## Fora de escopo (por enquanto)

- Notificação ativa (e-mail, push) de estoque baixo — o aviso é só
  visual, na própria tela de Estoque.
- Histórico de movimentações de estoque (quem tirou, quando, de qual
  OS) — só o saldo atual é mantido, sem log de auditoria.
- Corrigir/impedir a exclusão de uma peça do estoque que já foi usada
  em itens de OS existentes — pode ser excluída livremente; os itens
  antigos mantêm a referência (`EstoqueId`) mesmo que aponte pra um
  registro que não existe mais.
- Editar a `Quantidade`/`ValorUnitario` de um item de OS já adicionado
  não ajusta o estoque de volta — só criar/remover o item afeta o
  estoque (editar não existe hoje pros itens, só criar e remover).
- Categorias, fornecedor, unidade de medida, código/SKU da peça.
- Validar `Quantidade`/`ValorUnitario` negativos no cadastro/edição da
  peça — mesma decisão de fase já registrada na Constitution (sem
  FluentValidation ainda, válido pra todo o sistema, não só aqui). A
  única validação de quantidade desta feature é a de "estoque
  insuficiente" ao adicionar item de OS, que é uma regra de negócio
  específica, não validação de entrada genérica.
- Autorização por role para o CRUD de estoque (mesmo padrão atual da
  maioria dos módulos — sem restrição extra por enquanto).

## Suposições e Perguntas em Aberto

- Decidido: baixa automática de estoque ao adicionar item de OS vindo
  do estoque, com devolução ao remover o item ou excluir a OS.
- Decidido (revisão 2): campos expandidos — `Nome` (curto, obrigatório)
  e `Descricao` (detalhe, opcional) separados; `ValorUnitario` virou
  `PrecoVenda`; novo `PrecoCompra`; margem de lucro calculada, não
  gravada; `EstoqueMinimo` por peça, com aviso visual de estoque baixo
  na tela.
- Decidido (revisão 3, 2026-09-13): `EstoqueMinimo` removido — a
  oficina decidiu não usar o aviso de estoque baixo. Margem de lucro
  vira duas variantes calculadas (Markup e Venda), ambas continuam sem
  campo próprio no banco.
- Suposição: `Quantidade` do estoque e dos itens de OS são `decimal`
  (mesmo tipo já usado em `ItemOrdemServico.Quantidade`), permitindo
  frações (ex.: 0,5 litro de óleo), não só números inteiros.
