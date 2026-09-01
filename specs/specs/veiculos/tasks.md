# Tasks — Veículos

- [x] Criar entidade `Veiculo` em `Domain/Entities`
- [x] Criar `VeiculoConfiguration` (Fluent API, mapeamento snake_case, FK para `clientes`)
- [x] Registrar `DbSet<Veiculo>` no `AppDbContext`
- [x] Criar `VeiculoDto`
- [x] Criar `IVeiculoRepository`
- [x] Criar `VeiculoRepository`
- [x] Criar `CreateVeiculoCommand` + `CreateVeiculoHandler` (valida `ClienteId` via `IClienteRepository`)
- [x] Criar `UpdateVeiculoCommand` + `UpdateVeiculoHandler` (`UpdateVeiculoResult` enum: Success/VeiculoNotFound/ClienteInvalido)
- [x] Criar `DeleteVeiculoCommand` + `DeleteVeiculoHandler`
- [x] Criar `GetAllVeiculosQuery` + `GetAllVeiculosHandler`
- [x] Criar `GetVeiculoByIdQuery` + `GetVeiculoByIdHandler`
- [x] Criar `GetVeiculosByClienteIdQuery` + `GetVeiculosByClienteIdHandler`
- [x] Criar `VeiculosController` usando `IMediator`
- [x] Registrar `IVeiculoRepository` no `Program.cs`
- [x] Gerar migration `AddVeiculos`
- [x] Aplicar migration localmente (`dotnet ef database update`)
- [x] Buildar solução (`dotnet build`) e confirmar sem erros
- [x] Testar CRUD completo via API (curl):
  - [x] Criar veículo com `ClienteId` válido → `201`
  - [x] Criar veículo com `ClienteId` inexistente → `400`
  - [x] Buscar por id inexistente → `404`
  - [x] Listar todos os veículos → `200` com array
  - [x] Listar veículos por cliente → `200` com array (vazio se sem veículos)
  - [x] Atualizar veículo existente → `204`
  - [x] Atualizar com `ClienteId` inexistente → `400`
  - [x] Atualizar id inexistente → `404`
  - [x] Remover veículo → `204`; remover id inexistente → `404`

**Status: concluído.** Testado ponta a ponta via API real (cliente de teste criado e removido após os testes). Sem alteração em Clientes/Usuarios existentes.
