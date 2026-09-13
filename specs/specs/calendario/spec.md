# Spec — Calendário (Agendamentos)

**Data:** 2026-09-13 · **Status:** rascunho

> Feature nova, pedida na mesma rodada que removeu `PrevisaoTermino` de
> Ordens de Serviço (`specs/specs/ordens-servico/`) e passou a exigir
> que `TecnicoId` seja um usuário com `Role = "Mecânico"`. O Calendário
> é quem passa a responder "esse mecânico está livre nesse horário?".
>
> **Correção do usuário (frontend):** a tela de Calendário do
> `connectasys-hub` é só leitura — quem cria/edita/remove um
> Agendamento é exclusivamente o formulário de Ordens de Serviço, não
> uma tela própria de Calendário. Isso **não muda nada neste
> documento**: o CRUD completo (`Create`/`Update`/`Delete`) continua
> necessário no backend, só que o único consumidor dos endpoints de
> escrita passou a ser a tela de Ordens de Serviço em vez de uma tela
> de Calendário dedicada.

## Objetivo

Permitir que a oficina agende horários dos mecânicos, evitando que o
mesmo mecânico fique escalado para dois atendimentos ao mesmo tempo.
CRUD completo de agendamentos, mais um endpoint de checagem de
conflito consumido pela tela de Ordens de Serviço no momento de
escolher o técnico responsável.

## User stories

- Como atendente, quero agendar um horário para um mecânico atender um
  cliente/veículo (opcionalmente já vinculado a uma Ordem de Serviço).
- Como atendente, quero ver a agenda de um mecânico num dia, para saber
  os horários livres antes de agendar.
- Como atendente, ao escolher o técnico responsável numa OS, quero ser
  avisado se ele já tem outro agendamento no mesmo horário.
- Como atendente, quero editar ou cancelar um agendamento existente.

## Campos

### Agendamento

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Id | inteiro sequencial | sim (automático) | |
| TecnicoId | referência a Usuário | sim | o usuário referenciado precisa ter `Role = "Mecânico"` |
| ClienteId | referência a Cliente | não | agendamento pode existir antes de uma OS ser aberta |
| VeiculoId | referência a Veículo | não | |
| OrdemServicoId | referência a OrdemServico | não | preenchido quando o agendamento nasce do fluxo de criar/editar uma OS |
| DataHoraInicio | data/hora | sim | alinhada a um slot de 3 minutos (ver Suposições) |
| DataHoraFim | data/hora | não | se ausente, a duração é tratada como indefinida — só `DataHoraInicio` entra na checagem de conflito |
| Observacao | texto | não | anotação livre (ex.: "troca de óleo", "revisão dos 10.000km") |
| Status | texto (lista fixa) | sim | `Agendado` (padrão na criação), `Concluído`, `Cancelado` — só agendamentos `Agendado` contam pra checagem de conflito |
| DataCadastro | data/hora | não (automático) | preenchida na criação |

## Critérios de aceite

- Criar/atualizar agendamento exige `TecnicoId` de um usuário existente
  com `Role = "Mecânico"`; caso contrário, erro claro (`400`).
- `DataHoraInicio` é sempre alinhada ao slot de 3 minutos mais próximo
  (para baixo) antes de gravar — ver Suposições sobre o que isso
  significa na prática.
- Criar/atualizar um agendamento com `Status = "Agendado"` que
  conflita — mesmo `TecnicoId` **e** mesmo slot de `DataHoraInicio**
  que outro agendamento já `Agendado` — é rejeitado (`409 Conflict`),
  com os dados do agendamento existente no corpo da resposta (pra tela
  poder mostrar o modal de conflito sem precisar de uma segunda
  chamada).
- `GET /api/Agendamentos/conflito?tecnicoId=&dataHora=` retorna o
  agendamento `Agendado` do técnico informado no mesmo slot, se
  existir (`200` com o corpo), ou `204 No Content` se o horário estiver
  livre — usado pela tela de Ordens de Serviço **antes** de tentar
  salvar, pra mostrar o aviso de forma proativa (RF-09 de
  `specs/specs/ordens-servico/spec.md`).
- Listar agendamentos aceita filtro opcional por `tecnicoId` e por
  intervalo de datas (dia).
- Buscar/atualizar/remover agendamento com id inexistente retorna
  `404`.
- Remover um agendamento é definitivo (hard delete) — não existe
  "lixeira"/histórico de agendamentos removidos.

## Fora de escopo (por enquanto)

- Recorrência de agendamentos (ex.: "toda segunda às 9h").
- Notificação/lembrete ao técnico ou ao cliente.
- Feriados, folgas ou horário de expediente configurável por técnico —
  o sistema não sabe se um horário está "fora do expediente", só se já
  está ocupado por outro agendamento.
- Integração com calendário externo (Google Calendar, Outlook etc.).
- Arrastar-e-soltar para reagendar — a edição é só via formulário
  (CRUD comum).
- Duração real do atendimento além do par `DataHoraInicio`/
  `DataHoraFim` simples — nenhum cálculo de disponibilidade por faixa
  de horário (overlap), só por slot exato (ver Suposições).

## Suposições e Perguntas em Aberto

- **Pergunta em aberto — granularidade de 3 minutos:** o pedido
  original diz "o intervalo de horas disponível deve ser de 3
  minutos". Um atendimento de oficina mecânica tipicamente dura de 30
  minutos a várias horas, então 3 minutos é incomum como *duração* de
  um agendamento — mas faz sentido como *granularidade do horário de
  início* oferecido no seletor (ex.: 09:00, 09:03, 09:06, ... em vez
  de só 09:00, 09:30, 10:00). Este documento assume essa segunda
  leitura: **o slot de início é de 3 em 3 minutos**, e a duração do
  atendimento é outro assunto (campo `DataHoraFim`, opcional, sem
  granularidade própria). Se a intenção era outra (ex.: erro de
  digitação por "30 minutos", mais comum pra esse tipo de agenda),
  avisar antes da implementação — muda a UI da tela (uma grade de
  verdade de 3 em 3 minutos é bem mais densa que de 30 em 30).
- Suposição: a checagem de conflito compara **slots exatos** (mesmo
  `TecnicoId` + mesma `DataHoraInicio` truncada ao slot de 3 minutos),
  não sobreposição de intervalo (`DataHoraInicio`/`DataHoraFim` de um
  cruzando com o do outro). Mais simples de implementar e cobre o
  pedido literal ("mesmo dia e mesma hora"). Se dois agendamentos de
  durações diferentes puderem se sobrepor sem começar no mesmo slot
  exato, o sistema não detecta isso — sinalizar se overlap completo for
  necessário.
- Suposição: `ClienteId`, `VeiculoId` e `OrdemServicoId` são
  independentes e todos opcionais — um agendamento pode existir sem
  nenhum dos três (bloqueio manual de agenda) ou vinculado a uma OS
  específica.
- Suposição: o `409` de conflito ao criar/atualizar é uma segunda
  camada de proteção (o principal fluxo de uso é o frontend consultar
  `/conflito` antes de deixar o usuário salvar) — cobre o caso de duas
  pessoas agendando o mesmo mecânico ao mesmo tempo.
