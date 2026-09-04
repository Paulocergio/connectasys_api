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
