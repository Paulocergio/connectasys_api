# Tasks — Multitenant

## Fase 1 — Infraestrutura de tenant

- [x] `Core/Domain/Entities/Empresa.cs`
- [x] `Core/Application/Interfaces/Services/ITenantContext.cs`
- [x] `Infrastructure/Security/TenantContext.cs` (lê claim `empresa_id`
      via `IHttpContextAccessor`)
- [x] `Core/Application/Interfaces/Repositories/IEmpresaRepository.cs`
      + `Infrastructure/Persistence/Repositories/EmpresaRepository.cs`
      (`GetByIdAsync`, `AddAsync`, `CriarComPrimeiroUsuarioAsync`
      transacional)
- [x] `Infrastructure/Persistence/Configurations/EmpresaConfiguration.cs`
- [x] `Program.cs`: `AddHttpContextAccessor`, `ITenantContext`,
      `IEmpresaRepository`

## Fase 2 — `EmpresaId` em toda entidade de negócio

- [x] Adicionado `EmpresaId` em `Usuario`, `Cliente`, `Veiculo`,
      `OrdemServico`, `ItemOrdemServico`, `Estoque`, `ContaPagar`,
      `ContaReceber`, `Agendamento`
- [x] As 9 `Configuration.cs` correspondentes: coluna `empresa_id` +
      FK pra `Empresa` (`DeleteBehavior.Restrict`)
- [x] `ClienteConfiguration`: índices únicos de `Cpf`/`Cnpj` trocados
      de globais pra compostos `(EmpresaId, Cpf)`/`(EmpresaId, Cnpj)`

## Fase 3 — Repositórios escopados por tenant

- [x] `ClienteRepository`, `VeiculoRepository`, `ContaPagarRepository`,
      `ContaReceberRepository`, `EstoqueRepository`,
      `OrdemServicoRepository` (incluindo itens e
      `SalvarComContaReceberAsync`), `AgendamentoRepository`: injeta
      `ITenantContext`, filtra toda leitura, carimba `EmpresaId` em
      `AddAsync`
- [x] `UsuarioRepository`: injeta `ITenantContext`, filtra
      `GetAllAsync`/`GetByIdAsync`; `GetByEmailAsync` continua sem
      filtro (uso no login); `AddAsync` **não** carimba `EmpresaId`
      automaticamente (ver design.md)
- [x] `CreateUsuarioHandler`: injeta `ITenantContext`, carimba
      `EmpresaId` manualmente antes de `AddAsync`

## Fase 4 — Autenticação: JWT, login, cadastro self-service

- [x] `ITokenService`/`TokenService`: parâmetro `empresaId`, claim
      `"empresa_id"` no JWT
- [x] `LoginCommand`: novo `LoginStatus.TesteExpirado`
- [x] `LoginHandler`: injeta `IEmpresaRepository`, checa
      `TrialExpiraEm` antes de gerar token
- [x] `LoginResponseDto`: campo `TrialExpiraEmUtc`
- [x] Novo `Core/Application/Commands/Auth/Registrar/` —
      `RegistrarCommand`/`RegistrarHandler` (cria Empresa + primeiro
      Usuário Admin numa transação, trial de 3 dias, devolve token —
      auto-login)
- [x] `AuthController`: endpoint `POST /api/Auth/registrar`; `Login`
      mapeia `TesteExpirado` → `402`

## Fase 5 — Migration e dado existente

- [x] Migration `AddMultiTenant` gerada (`dotnet ef migrations add`)
- [x] Editada a mão: `INSERT` da empresa legada
      (`00000000-0000-0000-0000-000000000000`, trial em 100 anos)
      antes das `AddForeignKey`, pra bater com o `defaultValue` que o
      EF já usa no `AddColumn` de cada tabela
- [x] Backup do banco antes de aplicar
      (`backups/backup_connectasysdb_20260913_185258_pre_multitenant.sql`)
- [x] Migration aplicada localmente — confirmado sem erro de FK
      (empresa legada já existia quando as constraints foram criadas)
- [x] Confirmado: todo dado pré-existente (5 usuários, 2 clientes, 2
      veículos, 3 OS, 1 peça de estoque) associado corretamente à
      empresa legada, sem perda

## Fase 6 — Verificação ponta a ponta (curl, dados de teste removidos depois)

- [x] Cadastro self-service de duas empresas → tokens distintos,
      trial de 3 dias correto em ambas
- [x] Cadastro com e-mail repetido → `409`
- [x] Isolamento confirmado: empresa cria cliente → outra empresa não
      vê (lista vazia) → empresa original vê o seu
- [x] CPF repetido entre empresas diferentes → `201` nas duas (unicidade
      por empresa, não global)
- [x] `GET` de cliente de outra empresa por id → `404`
- [x] Técnico de uma empresa usado numa OS de outra empresa → `400`
      (cross-tenant bloqueado)
- [x] Trial expirado manualmente (`UPDATE` direto) → login → `402`
      com mensagem clara
- [x] `dotnet build` sem erros em todas as fases

## Fase 7 — Correção: trial por dia calendário + exclusão automática (revisão 2026-09-13)

- [x] `Core/Application/Common/Trial.cs` — `CalcularExpiracao` (dia
      calendário do cadastro + 2 dias, meia-noite UTC), substitui
      `AddDays(3)` corrido
- [x] `RegistrarHandler` atualizado pra usar `Trial.CalcularExpiracao`
- [x] `IEmpresaRepository`/`EmpresaRepository`: novo `ApagarTudoAsync`
      (transação, `DELETE` em ordem segura pelas 9 tabelas + empresa)
- [x] `LoginHandler`: chama `ApagarTudoAsync` no momento em que detecta
      trial vencido (antes só bloqueava)
- [x] `API/Program.cs`: middleware global pós-autenticação que checa
      `TrialExpiraEm` em toda requisição autenticada (cobre token
      emitido antes do vencimento, usado depois) e também dispara
      `ApagarTudoAsync`
- [x] Verificado: registro `22:19:16` do dia 13 → `trialExpiraEmUtc =
      2026-09-15T00:00:00Z`, batendo com o exemplo do usuário
- [x] Verificado: exclusão completa sem violação de FK com dataset
      realista (cliente, veículo, estoque, OS+item, agendamento, conta
      a pagar) em todas as 9 tabelas
- [x] Verificado: segunda tentativa de login pós-exclusão → mensagem
      genérica de credencial inválida (não revela que a conta existiu)
- [x] `dotnet build` sem erros

**Status geral: concluído e testado ponta a ponta.** Frontend
(`connectasys-hub`) consome via `specs/multitenant/` daquele
repositório, incluindo o modal de aceite de termos no cadastro
(Fase 7 do lado do hub).
