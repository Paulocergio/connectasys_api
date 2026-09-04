# Tasks — Contas a Receber

- [x] Reaproveitar `StatusConta` (criado em `contas-pagar`)
- [x] Criar entidade `ContaReceber` em `Domain/Entities`
- [x] Criar `ContaReceberConfiguration` (Fluent API, mapeamento snake_case, FK para `clientes`)
- [x] Registrar `DbSet<ContaReceber>` no `AppDbContext`
- [x] Criar `ContaReceberDto`
- [x] Criar `IContaReceberRepository`
- [x] Criar `ContaReceberRepository`
- [x] Criar `CreateContaReceberCommand` + `CreateContaReceberHandler` (valida `ClienteId` via `IClienteRepository`)
- [x] Criar `UpdateContaReceberCommand` + `UpdateContaReceberHandler` (`UpdateContaReceberResult` enum: Success/ContaNotFound/ClienteInvalido)
- [x] Criar `DeleteContaReceberCommand` + `DeleteContaReceberHandler`
- [x] Criar `GetAllContasReceberQuery` + `GetAllContasReceberHandler`
- [x] Criar `GetContaReceberByIdQuery` + `GetContaReceberByIdHandler`
- [x] Criar `GetContasReceberByClienteIdQuery` + `GetContasReceberByClienteIdHandler`
- [x] Criar `ContasReceberController` usando `IMediator`
- [x] Registrar `IContaReceberRepository` no `Program.cs`
- [x] Gerar migration (`20260901110634_AddContasPagarEContasReceber`, combinada com Contas a Pagar — as duas features foram implementadas na mesma sessão, então geramos uma única migration em vez de duas separadas como o `design.md` original previa)
- [x] Aplicar migration localmente (`dotnet ef database update`)
- [x] Buildar solução (`dotnet build`) e confirmar sem erros
- [x] Testar CRUD completo via API (curl):
  - [x] Criar conta com `ClienteId` válido → `201`, `Status` = "Pendente"
  - [x] Criar conta com `ClienteId` inexistente → `400`
  - [x] Buscar por id inexistente → `404`
  - [x] Listar todas → `200` com array
  - [x] Listar por cliente → `200` com array (vazio se sem contas / cliente inexistente)
  - [x] Atualizar informando `DataRecebimento` → `204`; buscar de novo → `Status` = "Paga"
  - [x] Atualizar com `ClienteId` inexistente → `400`
  - [x] Atualizar id inexistente → `404`
  - [x] Remover → `204`; remover id inexistente → `404`

**Status: concluído.** Testado ponta a ponta via API real (cliente de teste criado e removido após os testes).

## Correção pós-conclusão (2026-09-03)

Ao integrar o hub (`connectasys-hub`, feature `contas-a-receber`), o
`POST`/`PUT` retornavam `500` (`DbUpdateException` /
`Npgsql.../ArgumentException: Cannot write DateTime with Kind=Unspecified
to PostgreSQL type 'timestamp with time zone'`) ao enviar
`dataVencimento`/`dataRecebimento` no formato `yyyy-mm-dd` (o que
`<input type="date">` produz — `Kind=Unspecified` na desserialização).
`ContasPagar` não tinha esse problema porque `CreateContaPagarHandler`/
`UpdateContaPagarHandler` já chamavam `DateTime.SpecifyKind(...,
DateTimeKind.Utc)`; os handlers de `ContasReceber` não chamavam. Os
testes originais desta feature (`curl`) aparentemente usaram datas já em
UTC (com `Z`), por isso o teste "criar conta com `ClienteId` válido →
`201`" acima passou sem pegar o bug.

- [x] `CreateContaReceberHandler`: `DataVencimento` agora passa por
      `DateTime.SpecifyKind(..., DateTimeKind.Utc)`
- [x] `UpdateContaReceberHandler`: `DataVencimento` e `DataRecebimento`
      idem
- [x] Reconfirmado via `curl`: criar com `dataVencimento` `yyyy-mm-dd` →
      `201`; atualizar com `dataRecebimento` `yyyy-mm-dd` → `204`,
      `Status` = "Paga"; `ClienteId` inexistente no update → `400`;
      registro de teste removido após a verificação
