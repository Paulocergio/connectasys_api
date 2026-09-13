# Spec — Ordens de Serviço

> **Revisão 2026-09-13** (rodada de ajustes pedida pelo usuário): campo
> `PrevisaoTermino` removido (backend, DTO e coluna da tabela); a regra
> de `TecnicoId` só aceitar usuário com `Role = "Mecânico"`, antes fora
> de escopo, agora entra em vigor. Ver `specs/calendario/` (nova
> feature) para o CRUD de agenda dos mecânicos.

## Objetivo

Permitir que a oficina abra, acompanhe e feche Ordens de Serviço
(OS) — o documento central do negócio: registra o problema relatado
pelo cliente, o diagnóstico, o que foi feito, as peças usadas e o
valor cobrado, do momento em que o veículo entra até a entrega. Os
módulos de Clientes e Veículos já existem e servem de base pra este.

## User stories

- Como atendente, quero abrir uma OS pra um cliente e o veículo dele,
  com a descrição do problema relatado.
- Como mecânico, quero registrar o diagnóstico, a solução aplicada e
  as peças/materiais usados numa OS.
- Como atendente, quero acompanhar o status da OS (aberta, em
  andamento, aguardando peça, concluída, cancelada) e as datas de
  abertura, previsão e conclusão.
- Como financeiro, quero ver o valor total da OS (mão de obra +
  peças - desconto).
- Como dono da oficina, quero registrar quando o cliente aprovou o
  orçamento e quando recebeu o serviço concluído.

## Campos

### Ordem de Serviço

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Id | inteiro sequencial | sim (automático) | identificador único |
| ClienteId | referência a Cliente | sim | |
| VeiculoId | referência a Veículo | sim | veículo já tem `ClienteId`, então a OS herda o cliente por esse caminho — ver Suposições |
| TecnicoId | referência a Usuário | não | pode ser aberta sem técnico designado ainda |
| Status | texto (lista fixa) | sim | `Aberto` (padrão na criação), `Em Andamento`, `Aguardando Peça`, `Concluído`, `Cancelado` |
| DescricaoProblema | texto | sim | relato do cliente |
| Diagnostico | texto | não | preenchido depois da avaliação |
| Solucao | texto | não | o que foi feito |
| DataAbertura | data/hora | não (automático) | preenchida na criação |
| DataConclusao | data/hora | não | preenchida quando o status vira `Concluído` |
| ValorMaoDeObra | decimal | não (default 0) | |
| Desconto | decimal | não (default 0) | |
| AprovacaoClienteEm | data/hora | não | quando o cliente aprovou o orçamento |
| AprovacaoClienteNome | texto | não | nome de quem aprovou (registro simples, sem assinatura digital de verdade — ver Fora de Escopo) |

### Item da Ordem de Serviço (peça/material)

| Campo | Tipo | Obrigatório |
|---|---|---|
| Id | inteiro sequencial | sim |
| OrdemServicoId | referência à OS | sim |
| Descricao | texto | sim |
| Quantidade | decimal | sim |
| ValorUnitario | decimal | sim |

Valor total da OS = `ValorMaoDeObra + soma(itens.Quantidade × itens.ValorUnitario) − Desconto`, calculado na hora de retornar (não gravado como coluna própria — ver Fora de Escopo).

## Critérios de aceite

- Criar OS exige `ClienteId` e `VeiculoId` válidos (clientes/veículos
  que existem); `VeiculoId` inexistente ou de outro cliente retorna
  erro claro.
- OS nasce com `Status = "Aberto"` e `DataAbertura` preenchida
  automaticamente.
- Status só aceita um dos 5 valores da lista fixa; qualquer outro
  valor é rejeitado.
- Adicionar/remover itens (peças) é feito por endpoints próprios, não
  reescrevendo a OS inteira.
- Buscar OS por id retorna também a lista de itens e o valor total
  calculado.
- Listar OS retorna todas, com filtro opcional por cliente e por
  veículo (pra ver o histórico de um cliente/veículo específico).
- Remover uma OS remove os itens dela junto (não deixa item órfão).
- Criar/atualizar OS informando `TecnicoId` de um usuário cujo `Role`
  não é `"Mecânico"` retorna erro claro (`400`) — antes qualquer
  usuário podia ser designado, essa checagem passa a valer (revisão
  2026-09-13).

## Fora de escopo (por enquanto)

- Assinatura digital de verdade (captura de imagem/desenho) — por
  enquanto só um registro de "quem aprovou e quando", texto simples.
- Congelar (snapshot) o valor total no momento da conclusão — hoje é
  sempre calculado na hora, então se o preço de um item mudasse
  depois (não há edição de preço "histórico" ainda), o total
  recalcularia. Aceitável pro estágio atual.
- Geração de PDF/impressão da OS.
- Notificação ao cliente (e-mail/SMS) quando o status muda.
- Vínculo com estoque (baixa automática de peça usada) — os itens da
  OS são só um registro de texto livre, não descontam de um catálogo
  de estoque (que não existe ainda no sistema).

## Suposições e Perguntas em Aberto

- Suposição: `VeiculoId` é obrigatório (mesmo o negócio sendo oficina
  mecânica, o pedido original não citou veículo explicitamente, mas é
  o vínculo natural já preparado pelo módulo de Veículos — a spec de
  Veículos já cita "ordens de serviço vinculadas ao veículo" como
  próximo passo). Se a intenção era permitir OS sem veículo (ex.:
  serviço não automotivo), avisar pra eu ajustar.
- **Revisão 2026-09-13:** `PrevisaoTermino` removido — campo, coluna e
  qualquer referência no DTO saem do modelo (ver `design.md`). A regra
  de `TecnicoId` só aceitar `Role = "Mecânico"` (antes em "Fora de
  escopo") passa a valer; ver critério de aceite acima e `design.md`
  para onde a validação entra no `Create`/`UpdateOrdemServicoHandler`.
- **Pergunta em aberto:** a checagem de conflito de agenda (mesmo dia/
  horário do técnico) descrita em `specs/calendario/spec.md` acontece
  no momento de criar/atualizar a OS (validação no backend, rejeitando
  com `409` se houver conflito) ou é responsabilidade só do frontend
  (consulta prévia ao endpoint de conflito antes de deixar o usuário
  salvar, sem bloqueio no `Create`/`UpdateOrdemServicoHandler`)? Este
  documento assume a segunda opção — o backend não bloqueia, só expõe
  o endpoint de checagem consumido pela tela — pra não acoplar o CRUD
  de OS ao de Calendário; revisar se o negócio exige garantia no
  servidor (ex.: dois usuários salvando ao mesmo tempo).
