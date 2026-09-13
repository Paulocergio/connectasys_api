# Design — Multitenant

## Estratégia: `ITenantContext` no Repository, não parâmetro em cada Command

Em vez de acrescentar `EmpresaId` em todo `Command`/`Query` e fazer
cada `Handler` passar isso pro repositório (o jeito "livro-texto" de
CQRS), a empresa do usuário atual é lida uma vez por requisição e
injetada direto nos repositórios:

```csharp
// Core/Application/Interfaces/Services/ITenantContext.cs
public interface ITenantContext
{
    Guid EmpresaId { get; }
}

// Infrastructure/Security/TenantContext.cs
public class TenantContext : ITenantContext
{
    public Guid EmpresaId { get; }

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        var claim = httpContextAccessor.HttpContext?.User?.FindFirst("empresa_id")?.Value;
        EmpresaId = Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }
}
```

Cada repositório (`ClienteRepository`, `VeiculoRepository`,
`OrdemServicoRepository`, `EstoqueRepository`, `ContaPagarRepository`,
`ContaReceberRepository`, `AgendamentoRepository`, `UsuarioRepository`)
injeta `ITenantContext` no construtor, guarda `_empresaId`, e:

- Todo método de leitura (`GetAllAsync`, `GetByIdAsync`, `GetByXAsync`)
  filtra `.Where(x => x.EmpresaId == _empresaId)`.
- Todo `AddAsync` carimba `entity.EmpresaId = _empresaId` antes de
  salvar — **o Handler que cria a entidade nunca precisa saber a
  empresa**, exceto os dois casos especiais abaixo.

**Por que esse caminho em vez de parâmetro explícito:** o mesmo
resultado (nenhuma consulta escapa sem filtro), só que o "esquecimento"
vira impossível de cometer sem também esquecer de injetar o serviço
(erro de compilação óbvio), em vez de um `EmpresaId` que um Handler
poderia simplesmente não usar. Também reduz o raio de mudança de
"todo Command/Query/Handler/Controller do sistema" pra "só a camada de
repositório" — ~9 arquivos em vez de ~60.

## Os dois casos especiais: `Usuario` e cadastro self-service

`UsuarioRepository.AddAsync` **não** carimba `EmpresaId`
automaticamente — quem chama precisa já ter definido o campo:

- `CreateUsuarioHandler` (adicionar colega de equipe, autenticado):
  injeta `ITenantContext` e seta `EmpresaId = _tenantContext.EmpresaId`
  antes de chamar `AddAsync`.
- `RegistrarHandler` (cadastro self-service, **anônimo** — não existe
  tenant ambiente ainda, é o próprio cadastro que cria a empresa): seta
  `EmpresaId = empresaRecemCriada.Id` diretamente.

Se `UsuarioRepository.AddAsync` carimbasse `_tenantContext.EmpresaId`
como os outros repositórios, o cadastro self-service quebraria (sem
token, `ITenantContext.EmpresaId` resolve pra `Guid.Empty`).

`UsuarioRepository.GetByEmailAsync` também **não** é filtrado por
empresa — é o método usado no login, que é como o sistema descobre a
qual empresa o usuário pertence; não pode já exigir saber a empresa
antes de saber.

## Entidade `Empresa`

```csharp
public class Empresa
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public DateTime TrialExpiraEm { get; set; }
}
```

Tabela `empresas`, sem escopo de tenant nela mesma (óbvio — é a própria
unidade de tenant). `EmpresaRepository` não injeta `ITenantContext`.

## Cadastro self-service — transação Empresa + primeiro Usuário

```csharp
public interface IEmpresaRepository
{
    Task<Empresa?> GetByIdAsync(Guid id);
    Task AddAsync(Empresa empresa);
    Task CriarComPrimeiroUsuarioAsync(Empresa empresa, Usuario usuario);
    Task ApagarTudoAsync(Guid empresaId);
}
```

`CriarComPrimeiroUsuarioAsync` grava os dois numa transação só (mesmo
padrão de `OrdemServicoRepository.SalvarComContaReceberAsync`) — uma
Empresa nunca fica órfã (criada sem nenhum usuário) se algo falhar no
meio do caminho.

`RegistrarHandler`:

```
1. GetByEmailAsync(request.Email) → se existir, EmailEmUso
2. Monta Empresa { TrialExpiraEm = Trial.CalcularExpiracao(UtcNow) }
3. Monta Usuario { EmpresaId = empresa.Id, Role = Roles.Admin, ... }
4. CriarComPrimeiroUsuarioAsync(empresa, usuario)
5. GerarToken(...) — mesmo shape de resposta do login (auto-login)
```

## `Trial.CalcularExpiracao` — vencimento por dia calendário

Correção pedida pelo usuário: a primeira versão usava
`DataCadastro.AddDays(3)` (72h exatas a partir do horário do cadastro).
O usuário deu um exemplo concreto — "cadastrar hoje dia 13, dia 15 tem
que expirar" — que é matemática de **dia calendário**, não de horas:

```csharp
// Core/Application/Common/Trial.cs
public static class Trial
{
    private const int DiasAdicionaisAposCadastro = 2;

    public static DateTime CalcularExpiracao(DateTime dataCadastroUtc) =>
        dataCadastroUtc.Date.AddDays(DiasAdicionaisAposCadastro);
}
```

`.Date` zera a hora (meia-noite UTC do dia do cadastro), depois soma 2
dias — cadastro em qualquer horário do dia 13 vira `15/09 00:00 UTC`,
que é exatamente o instante em que o dia 15 começa. Verificado via
curl: cadastro às `2026-09-13 22:19:16` → `trialExpiraEmUtc:
2026-09-15T00:00:00Z`.

Usado tanto em `RegistrarHandler` quanto em qualquer outro lugar que
precise calcular vencimento a partir de uma `DataCadastro` — hoje só
tem esse um chamador.

## JWT — claim `empresa_id`

`ITokenService.GerarToken` ganhou um parâmetro `Guid empresaId`, que
vira um claim customizado `"empresa_id"` no token (junto de `sub`,
`email`, `name`, `role` que já existiam). É esse claim que
`TenantContext` lê em toda requisição autenticada.

## Login — checagem de trial e exclusão imediata

`LoginHandler`, depois de validar a senha e antes de gerar o token:

```csharp
var empresa = await _empresaRepository.GetByIdAsync(usuario.EmpresaId);
if (empresa is not null && DateTime.UtcNow > empresa.TrialExpiraEm)
{
    await _empresaRepository.ApagarTudoAsync(empresa.Id);
    return new LoginResultado { Status = LoginStatus.TesteExpirado };
}
```

`AuthController` mapeia `TesteExpirado` pra `402 Payment Required` com
mensagem clara. `LoginResponseDto` ganhou `TrialExpiraEmUtc` (devolvido
tanto no login quanto no cadastro), pro frontend guardar na sessão —
sem nenhuma UI consumindo isso ainda além de guardar (ver spec, Fora
de Escopo).

## `EmpresaRepository.ApagarTudoAsync` — exclusão definitiva

Chamado no exato momento em que o vencimento é detectado (login ou o
middleware abaixo) — sem confirmação adicional, sem soft-delete, sem
job em segundo plano:

```csharp
public async Task ApagarTudoAsync(Guid empresaId)
{
    await using var transaction = await _context.Database.BeginTransactionAsync();

    await _context.Database.ExecuteSqlInterpolatedAsync(
        $"DELETE FROM itens_ordem_servico WHERE empresa_id = {empresaId}");
    await _context.Database.ExecuteSqlInterpolatedAsync(
        $"DELETE FROM agendamentos WHERE empresa_id = {empresaId}");
    await _context.Database.ExecuteSqlInterpolatedAsync(
        $"DELETE FROM contas_receber WHERE empresa_id = {empresaId}");
    await _context.Database.ExecuteSqlInterpolatedAsync(
        $"DELETE FROM contas_pagar WHERE empresa_id = {empresaId}");
    await _context.Database.ExecuteSqlInterpolatedAsync(
        $"DELETE FROM ordens_servico WHERE empresa_id = {empresaId}");
    await _context.Database.ExecuteSqlInterpolatedAsync(
        $"DELETE FROM veiculos WHERE empresa_id = {empresaId}");
    await _context.Database.ExecuteSqlInterpolatedAsync(
        $"DELETE FROM clientes WHERE empresa_id = {empresaId}");
    await _context.Database.ExecuteSqlInterpolatedAsync(
        $"DELETE FROM estoque WHERE empresa_id = {empresaId}");
    await _context.Database.ExecuteSqlInterpolatedAsync(
        $"DELETE FROM usuarios WHERE empresa_id = {empresaId}");
    await _context.Database.ExecuteSqlInterpolatedAsync(
        $"DELETE FROM empresas WHERE id = {empresaId}");

    await transaction.CommitAsync();
}
```

Ordem das `DELETE`s respeita as foreign keys — filhos antes dos pais
(`itens_ordem_servico` antes de `ordens_servico`, e as 8 tabelas de
negócio antes de `usuarios`, que vem antes de `empresas`). Tudo numa
transação só: ou apaga tudo, ou (em caso de erro no meio) não apaga
nada — nunca fica pela metade. Raw SQL via
`ExecuteSqlInterpolatedAsync` (não LINQ/tracked entities) porque é
volume potencialmente grande e não precisa carregar nada em memória, só
apagar; o `{empresaId}` interpolado vira parâmetro, não concatenação de
string (sem risco de SQL injection).

Efeito colateral esperado e verificado: uma segunda tentativa de login
com as mesmas credenciais depois da exclusão cai em
`GetByEmailAsync` retornando `null` → mensagem genérica "Email ou senha
inválidos" (não um "conta não existe mais" que revelaria o que
aconteceu) — mesmo comportamento de qualquer credencial inválida.

## Middleware — cobre requisições feitas com token já emitido

O JWT tem validade de até 60 minutos. Sem isso, um token emitido pouco
antes do vencimento do trial continuaria sendo aceito por até 1h depois
do vencimento em qualquer endpoint (não só login). `API/Program.cs`
registra um middleware global, depois de `UseAuthentication()` e antes
de `UseAuthorization()`:

```csharp
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var claim = context.User.FindFirst("empresa_id")?.Value;
        if (Guid.TryParse(claim, out var empresaId))
        {
            var empresaRepository = context.RequestServices.GetRequiredService<IEmpresaRepository>();
            var empresa = await empresaRepository.GetByIdAsync(empresaId);
            if (empresa is not null && DateTime.UtcNow > empresa.TrialExpiraEm)
            {
                await empresaRepository.ApagarTudoAsync(empresaId);
                context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Seu período de teste expirou. Entre em contato para continuar usando o ConnectaSys."
                });
                return;
            }
        }
    }

    await next();
});
```

Resolve `IEmpresaRepository` via `context.RequestServices` (não injeção
no construtor) porque middlewares inline (`app.Use(...)`) são
singletons por natureza — não dá pra injetar um serviço `Scoped` direto
no delegate, precisa pegar do container por requisição. Roda em
**toda** requisição autenticada, então repete a mesma checagem que já
existe no login — redundante para o caso comum (login recente, trial
válido), mas é exatamente essa redundância que fecha a janela do token
antigo.

## Unicidade de CPF/CNPJ — de global pra por empresa

```csharp
builder.HasIndex(c => new { c.EmpresaId, c.Cpf }).IsUnique();
builder.HasIndex(c => new { c.EmpresaId, c.Cnpj }).IsUnique();
```

Antes era `HasIndex(c => c.Cpf).IsUnique()` (global) — duas oficinas
diferentes com o mesmo cliente real (mesmo CPF) davam conflito, o que
não devia acontecer entre tenants distintos. Postgres trata múltiplos
`NULL` como não-conflitantes num índice único por padrão, então clientes
sem documento continuam permitidos livremente dentro da mesma empresa.

E-mail de usuário **não** mudou — continua único globalmente
(`HasIndex(u => u.Email).IsUnique()`), porque é por ele que o login
encontra a empresa certa.

## Migration — dado existente vira uma empresa "legada"

`AddMultiTenant`: `AddColumn` de `empresa_id` (Guid, `NOT NULL`) em
todas as 9 tabelas, cada uma com `defaultValue:
00000000-0000-0000-0000-000000000000` (gerado automaticamente pelo EF
por ser o `default(Guid)`) — isso faz **toda linha já existente** ser
carimbada com esse id automaticamente, sem violar a constraint `NOT
NULL` na hora de aplicar.

Antes de criar as foreign keys (que exigem o registro em `empresas`
existir), a própria migration insere a empresa legada com esse mesmo
id fixo:

```sql
INSERT INTO empresas (id, nome, data_cadastro, trial_expira_em)
VALUES (
    '00000000-0000-0000-0000-000000000000',
    'ConnectaSys (dados anteriores ao multitenant)',
    now(),
    now() + interval '100 years'
);
```

Trial 100 anos no futuro — ninguém que já usava o sistema antes desta
feature fica bloqueado pela regra de teste de 3 dias (essa regra só
vale pra empresa nova, criada via `/api/Auth/registrar`).

## Arquivos alterados/criados

- Novo: `Core/Domain/Entities/Empresa.cs`
- Novo: `Core/Application/Interfaces/Services/ITenantContext.cs`
- Novo: `Infrastructure/Security/TenantContext.cs`
- Novo: `Core/Application/Interfaces/Repositories/IEmpresaRepository.cs`
  + `Infrastructure/Persistence/Repositories/EmpresaRepository.cs`
  (inclui `ApagarTudoAsync`)
- Novo: `Infrastructure/Persistence/Configurations/EmpresaConfiguration.cs`
- Novo: `Core/Application/Common/Trial.cs` — `CalcularExpiracao` (dia
  calendário do cadastro + 2 dias)
- Novo: `Core/Application/Commands/Auth/Registrar/` (`RegistrarCommand`
  + `RegistrarHandler`)
- `Core/Domain/Entities/*.cs` (9 entidades) — `EmpresaId` novo
- `Infrastructure/Persistence/Configurations/*.cs` (9 configs) —
  mapeamento da coluna + FK pra `Empresa`; `ClienteConfiguration`
  também troca os índices únicos de Cpf/Cnpj pra compostos
- `Infrastructure/Persistence/Repositories/*.cs` (9 repositórios) —
  injeção de `ITenantContext`, filtro em toda leitura, carimbo em
  `AddAsync`
- `Core/Application/Commands/Usuarios/CreateUsuario/CreateUsuarioHandler.cs`
  — injeta `ITenantContext`, carimba `EmpresaId` manualmente
- `Core/Application/Interfaces/Services/ITokenService.cs` +
  `Infrastructure/Security/TokenService.cs` — parâmetro `empresaId` +
  claim `"empresa_id"`
- `Core/Application/Commands/Auth/Login/LoginCommand.cs` +
  `LoginHandler.cs` — status `TesteExpirado`, checagem de trial +
  chamada a `ApagarTudoAsync`
- `Core/Application/DTOs/LoginResponseDto.cs` — campo
  `TrialExpiraEmUtc`
- `API/Controllers/AuthController.cs` — endpoint `POST
  /api/Auth/registrar`, resposta `402` pro trial expirado
- `API/Program.cs` — `AddHttpContextAccessor`, registro de
  `ITenantContext`/`TenantContext` e `IEmpresaRepository`/`EmpresaRepository`,
  middleware global de checagem de trial (depois de
  `UseAuthentication()`, antes de `UseAuthorization()`)
- Migration `AddMultiTenant` (+ backfill via `migrationBuilder.Sql`)

## Riscos e decisões

- **Decisão:** bloqueio total de login após o trial (não modo
  só-leitura) — confirmado com o usuário; mais simples de implementar
  corretamente (um `if` no login) do que bloquear escrita em todo
  controller.
- **Decisão (revisão 2026-09-13):** a exclusão dos dados é automática e
  imediata no momento em que o vencimento é detectado (login ou
  middleware), não uma ação manual posterior — corrigido a pedido do
  usuário, que também exigiu um contrato de aceite explícito no
  cadastro avisando disso (ver `connectasys-hub/specs/multitenant/`
  pro modal). Substitui a decisão anterior de só bloquear o acesso e
  manter o dado guardado.
- **Decisão:** vencimento do trial é por **dia calendário**
  (`Trial.CalcularExpiracao`), não por 72h exatas a partir do horário
  do cadastro — corrigido a pedido do usuário com um exemplo concreto
  (cadastro dia 13 → expira ao virar dia 15).
- **Decisão:** sem transação cross-repository além do cadastro
  self-service e da exclusão — o resto das operações (criar cliente,
  criar OS etc.) continua um único `SaveChangesAsync` por repositório,
  mesmo padrão pragmático já usado no resto do projeto.
- **Risco aceito:** liberar uma empresa antes do trial expirar (upgrade
  pra assinatura paga) é manual (`UPDATE empresas SET
  trial_expira_em = ...`) — sem integração de cobrança, documentado
  como fora de escopo. Diferente da exclusão em si, que agora é
  automática.
- **Risco aceito, deliberado:** a exclusão não tem nenhuma janela de
  confirmação ou desfazimento — é exatamente o que o contrato de
  aceite no cadastro avisa antecipadamente. Um usuário que ignora o
  aviso e deixa o trial vencer perde os dados sem chance de reaver.
- **Risco aceito:** um Admin de uma empresa pode, em teoria, adivinhar
  ids sequenciais (int) de registros de outra empresa e tentar acessá-los
  — mitigado porque toda consulta por id já filtra por `EmpresaId`
  (retorna `404`, nunca vaza dado), mas o espaço de ids continua
  compartilhado entre empresas (não reinicia em 1 pra cada tenant).
  Comportamento aceitável — é assim que MUITO SaaS multitenant com id
  incremental funciona (o dado nunca vaza, só o padrão de crescimento
  do id é observável).

## Estratégia de verificação

Testado via curl, ponta a ponta, contra o Postgres local (dados de
teste removidos depois):

- Cadastro self-service de duas empresas diferentes → duas contas
  distintas, cada uma com token próprio, trial correto.
- Cadastro com e-mail repetido → `409`, nada criado.
- Empresa A cria um cliente → Empresa B lista clientes → lista vazia
  (não vê o da A). Empresa A lista → vê o seu.
- Empresa B cria cliente com o **mesmo CPF** do cliente da Empresa A →
  `201` (unicidade é por empresa, confirmado).
- Empresa B tenta `GET /api/Clientes/{id}` de um cliente da Empresa A →
  `404`.
- Empresa A cria um usuário Mecânico; Empresa B tenta criar uma OS
  usando esse `tecnicoId` → `400` (técnico inválido, cross-tenant
  bloqueado).
- `UPDATE` manual do `trial_expira_em` de uma empresa pro passado →
  login dessa empresa → `402` com mensagem clara.
- Dado que já existia antes da migration (5 usuários, 2 clientes, 2
  veículos, 3 OS, 1 peça de estoque) continua todo associado à empresa
  legada, intacto, sem nenhuma linha perdida ou duplicada.
- `dotnet build` sem erros em todas as etapas.

### Verificação adicional — correção do trial (revisão 2026-09-13)

- Registro às `2026-09-13 22:19:16` UTC → `trialExpiraEmUtc` retornado
  = `2026-09-15T00:00:00Z`, batendo exatamente com o exemplo do usuário
  (cadastrou dia 13, expira ao virar dia 15) — confirma
  `Trial.CalcularExpiracao` (dia calendário do cadastro + 2 dias, não
  72h corridas).
- Populada uma empresa de teste com dado realista em todas as 9
  tabelas (cliente, veículo, peça de estoque, OS com item vinculado à
  peça, agendamento, conta a pagar) e forçado `trial_expira_em` pro
  passado → login → `402` + `ApagarTudoAsync` executa sem nenhuma
  violação de FK, ordem de `DELETE` corretamente respeitando as
  dependências.
- Segunda tentativa de login com as mesmas credenciais, depois da
  exclusão → `401`/mensagem genérica "Email ou senha inválidos" (não
  revela que a conta existiu e foi apagada) — `GetByEmailAsync` não
  encontra nada, comportamento idêntico a qualquer credencial inválida.
- Confirmado que uma empresa real (não-teste) já existente no banco
  local não foi tocada em nenhum momento da verificação — só empresas
  criadas especificamente pra este teste foram usadas/apagadas.
- `dotnet build` sem erros depois de todas as mudanças.
