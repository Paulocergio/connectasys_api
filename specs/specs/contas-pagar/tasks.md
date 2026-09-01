# Tasks — Contas a Pagar

- [x] Criar `StatusConta` em `Core/Application/Common/StatusConta.cs` (compartilhado com Contas a Receber)
- [x] Criar entidade `ContaPagar` em `Domain/Entities`
- [x] Criar `ContaPagarConfiguration` (Fluent API, mapeamento snake_case)
- [x] Registrar `DbSet<ContaPagar>` no `AppDbContext`
- [x] Criar `ContaPagarDto`
- [x] Criar `IContaPagarRepository`
- [x] Criar `ContaPagarRepository`
- [x] Criar `CreateContaPagarCommand` + `CreateContaPagarHandler`
- [x] Criar `UpdateContaPagarCommand` + `UpdateContaPagarHandler`
- [x] Criar `DeleteContaPagarCommand` + `DeleteContaPagarHandler`
- [x] Criar `GetAllContasPagarQuery` + `GetAllContasPagarHandler`
- [x] Criar `GetContaPagarByIdQuery` + `GetContaPagarByIdHandler`
- [x] Criar `ContasPagarController` usando `IMediator`
- [x] Registrar `IContaPagarRepository` no `Program.cs`
- [x] Gerar migration (combinada com Contas a Receber: `AddContasPagarEContasReceber`, ver `contas-receber/tasks.md`)
- [x] Aplicar migration localmente (`dotnet ef database update`)
- [x] Buildar solução (`dotnet build`) e confirmar sem erros
- [x] Testar CRUD completo via API (curl):
  - [x] Criar conta → `201`, `Status` = "Pendente"
  - [x] Criar conta com vencimento passado → `Status` = "Atrasada"
  - [x] Buscar por id inexistente → `404`
  - [x] Listar todas → `200` com array
  - [x] Atualizar informando `DataPagamento` → `204`; buscar de novo → `Status` = "Paga"
  - [x] Atualizar id inexistente → `404`
  - [x] Remover → `204`; remover id inexistente → `404`

**Status: concluído.** Testado ponta a ponta via API real contra o Postgres local. Migration gerada em conjunto com Contas a Receber (mesma sessão de trabalho).
