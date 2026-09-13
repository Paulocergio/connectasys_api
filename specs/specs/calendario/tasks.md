# Tasks — Calendário (Agendamentos)

## Fase 1 — Banco de dados

- [x] Criar `Core/Application/Common/StatusAgendamento.cs`
- [x] Criar `Core/Application/Common/SlotAgendamento.cs` (truncamento
      pro slot de 3 minutos)
- [x] Criar entidade `Core/Domain/Entities/Agendamento.cs`
- [x] Criar `Infrastructure/Persistence/Configurations/AgendamentoConfiguration.cs`
      (mapeamento, FKs, índice composto `tecnico_id, data_hora_inicio`)
- [x] `AppDbContext`: `DbSet<Agendamento> Agendamentos`
- [x] Gerar e aplicar migration (`20260913145259_AddAgendamentos`)
- [x] `dotnet build` sem erros; API sobe normal (confirmado subindo a
      API localmente e checando `/swagger/v1/swagger.json`)

**Fase 1: concluída.**

## Fase 2 — CQRS e endpoints

- [x] `AgendamentoDto` (com `DaEntidade`)
- [x] `IAgendamentoRepository` + `AgendamentoRepository` (inclui
      `GetByTecnicoIdAsync` com filtro opcional de dia, e
      `GetConflitoAsync` com `ignorarId` opcional)
- [x] `CreateAgendamentoCommand`/`Handler` (valida técnico é Mecânico,
      trunca slot, rejeita conflito com `409` e o agendamento
      conflitante no corpo)
- [x] `UpdateAgendamentoCommand`/`Handler` (mesma validação, ignorando
      o próprio id na checagem de conflito; também valida `Status`)
- [x] `DeleteAgendamentoCommand`/`Handler`
- [x] `GetAllAgendamentosQuery`/`Handler`
- [x] `GetAgendamentoByIdQuery`/`Handler`
- [x] `GetAgendamentosByTecnicoIdQuery`/`Handler`
- [x] `GetConflitoAgendamentoQuery`/`Handler`
- [x] `AgendamentosController` com `[Authorize]` na classe e
      `[Authorize(Roles = Admin,Mecânico,Recepcionista)]` nas escritas
      (mesmo padrão de `OrdensServicoController`, que já diverge do
      "sem restrição de role" registrado no `design.md` original —
      autorização por role foi adicionada numa feature posterior não
      refletida nas specs antigas; seguido o padrão real do código)
- [x] Registrar `IAgendamentoRepository` no `Program.cs`
- [x] `dotnet build` sem erros

**Fase 2: concluída.**

## Fase 3 — Verificação

- [x] Testado via curl (usuário Mecânico de teste, token real): criar
      (`201`), criar conflitante mesmo slot (`409` com corpo do
      agendamento existente), criar 3 minutos depois — slot diferente
      (`201`, sem conflito)
- [x] Testado `GET /api/Agendamentos/conflito` — slot ocupado (`200`
      com o agendamento), slot livre (`204`)
- [x] Testado criar com técnico Admin (não-Mecânico) → `400`
- [x] Testado editar sem mudar horário (não conflita consigo mesmo,
      `204`) e editar mudando pra slot ocupado por outro agendamento
      (`409`)
- [x] Testado remover (`204`) e `GET` por id depois (`404`)
- [x] Testado listar por técnico filtrando por dia — retorna só os do
      dia certo
- [x] Todos os registros de teste (2 usuários, agendamentos, 1 OS, 1
      peça de estoque) removidos depois; confirmado no banco (`SELECT
      count(*)` = 0 em cada tabela envolvida)

**Fase 3: concluída.**

**Status geral: concluído.** Testado ponta a ponta via API local real
(Postgres local), cobrindo CRUD completo de Agendamentos, a
granularidade de slot de 3 minutos, a checagem de conflito (tanto o
endpoint dedicado quanto o bloqueio em `Create`/`Update`), e a
validação de técnico exigindo perfil Mecânico.

## Dependência cruzada

- [x] `specs/specs/ordens-servico/`: `TecnicoId` validado contra
      `Role = "Mecânico"` (implementado e testado junto desta rodada,
      reaproveita a mesma checagem de usuário desta feature)
