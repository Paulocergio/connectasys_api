# Tasks — Ordens de Serviço

## Fase 1 — Banco de dados (pedida primeiro)

- [x] Criar `Core/Application/Common/StatusOrdemServico.cs`
- [x] Criar entidades `Core/Domain/Entities/OrdemServico.cs` e
      `ItemOrdemServico.cs`
- [x] Criar `Infrastructure/Persistence/Configurations/OrdemServicoConfiguration.cs`
      e `ItemOrdemServicoConfiguration.cs` (mapeamento, FKs, tipos)
- [x] Gerar migration (`20260904210552_AddOrdensServico`)
- [x] Aplicar migration localmente
- [x] `dotnet build` sem erros; API sobe normal

**Fase 1: concluída.**

## Fase 2 — CQRS e endpoints

- [x] `OrdemServicoDto`/`ItemOrdemServicoDto` (com `OrdemServicoDto.DaEntidade`
      calculando `ValorTotal`)
- [x] `IOrdemServicoRepository` + implementação (inclui métodos de item:
      `GetItemByIdAsync`/`AddItemAsync`/`RemoveItemAsync`)
- [x] `CreateOrdemServicoCommand`/`Handler` (valida cliente, veículo
      pertence ao cliente, técnico existe)
- [x] `UpdateOrdemServicoCommand`/`Handler` (valida status, cliente,
      veículo, técnico)
- [x] `DeleteOrdemServicoCommand`/`Handler`
- [x] `AddItemOrdemServicoCommand`/`Handler`
- [x] `RemoveItemOrdemServicoCommand`/`Handler`
- [x] `GetAllOrdensServicoQuery`/`Handler`
- [x] `GetOrdemServicoByIdQuery`/`Handler` (com itens + valor total)
- [x] `GetOrdensServicoByClienteIdQuery`/`Handler`
- [x] `GetOrdensServicoByVeiculoIdQuery`/`Handler`
- [x] `OrdensServicoController` com `[Authorize]`
- [x] Registrar `IOrdemServicoRepository` no `Program.cs`
- [x] `dotnet build` sem erros

**Fase 2: concluída.**

## Fase 3 — Verificação

- [x] Testar via curl (token válido): criar (`201`), listar, buscar
      por id (com itens e valor total corretos: 150 + 80 + 4×25 = 330),
      atualizar status pra "Concluído" com desconto (valor recalculado
      pra 320), adicionar/remover item, filtrar por cliente/veículo,
      remover OS (itens somem junto via cascade, `GET` depois → `404`)
- [x] Validações de erro: veículo de outro cliente → `400`; status
      inválido (`"Pronto"`) → `400`
- [x] Registros de teste removidos depois (OS, itens e veículo de
      teste)

**Fase 3: concluída.**

**Status geral: concluído.** Testado ponta a ponta via API real,
incluindo o cálculo de valor total, a validação cruzada
cliente/veículo, e o cascade de remoção de itens.

## Bug encontrado depois da integração com o front

- [x] `POST /api/OrdensServico` com `previsaoTermino` preenchido (ex.:
      `"2026-09-10"`, formato que `<input type="date">` manda, sem
      fuso) dava `500`: Npgsql rejeita `DateTime` com
      `Kind=Unspecified` em coluna `timestamptz`. Bug estrutural — não
      específico da OS, qualquer campo `DateTime?` do sistema estava
      exposto (ex.: datas de Contas a Pagar/Receber também usam
      `<input type="date">` no front).
- [x] Corrigido de forma global em
      `Infrastructure/Persistence/Context/AppDbContext.cs`:
      `UtcDateTimeConverter` aplicado via `ConfigureConventions` a
      toda propriedade `DateTime`/`DateTime?` do modelo — normaliza
      pra `Kind=Utc` antes de gravar, sem precisar tocar em cada
      handler.
- [x] Nenhuma migration necessária (`dotnet ef migrations add
      --dry-run` confirmou: sem mudança de schema, só conversão em
      runtime)
- [x] Testado: `previsaoTermino: "2026-09-10"` (sem `Z`, sem hora) →
      `201`, valor salvo corretamente; registro de teste removido

## Ajustes pendentes (rodada de specs 2026-09-13)

- [x] Remover `PrevisaoTermino` de `OrdemServico.cs`,
      `OrdemServicoConfiguration.cs`, `OrdemServicoDto.cs`,
      `Create`/`UpdateOrdemServicoCommand`
  - Critério de pronto: `dotnet build` sem erros; nenhuma referência
    a `PrevisaoTermino`/`previsao_termino` restante no projeto
    (confirmado — só resta em migrations antigas, que preservam
    histórico por design)

- [x] Gerar e aplicar migration derrubando a coluna
      `previsao_termino` de `ordens_servico`
  - Migration `20260913144812_RemovePrevisaoTerminoDeOrdensServico`,
    aplicada localmente (`ALTER TABLE ordens_servico DROP COLUMN
    previsao_termino` confirmado no log do `dotnet ef database update`)

- [x] Validar `TecnicoId` contra `Role = "Mecânico"` em
      `Create`/`UpdateOrdemServicoHandler` (o enum `TecnicoInvalido` já
      existia em `UpdateOrdemServicoResult`, reaproveitado; `Create`
      passou a checar a role junto da existência do usuário)
  - Testado via curl: `tecnicoId` de usuário Admin → `400`
    ("ClienteId, VeiculoId ou TecnicoId inválido." no Create; "TecnicoId
    inválido." no Update); `tecnicoId` de usuário Mecânico → `201`
    normalmente; sem `tecnicoId` → continua opcional

- [x] Testado via curl: os dois cenários acima end-to-end (usuário de
      teste Mecânico criado, OS criada com sucesso, `GET` confirmado
      sem `previsaoTermino` no payload, dados de teste removidos
      depois)
