# Design — Ordens de Serviço

> **Revisão 2026-09-13:** `PrevisaoTermino` removida da entidade, DTO,
> tabela (migration nova) e commands; `TecnicoId` passa a ser validado
> contra `Role = "Mecânico"` em `Create`/`UpdateOrdemServicoHandler`.

## Entidades (`Core/Domain/Entities/`)

```csharp
public class OrdemServico
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int VeiculoId { get; set; }
    public Guid? TecnicoId { get; set; }
    public string Status { get; set; } = StatusOrdemServico.Aberto;
    public string DescricaoProblema { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? Solucao { get; set; }
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataConclusao { get; set; }
    public decimal ValorMaoDeObra { get; set; }
    public decimal Desconto { get; set; }
    public DateTime? AprovacaoClienteEm { get; set; }
    public string? AprovacaoClienteNome { get; set; }

    public List<ItemOrdemServico> Itens { get; set; } = new();
}

public class ItemOrdemServico
{
    public int Id { get; set; }
    public int OrdemServicoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}
```

`Itens` é a única propriedade de navegação do projeto até agora (as
outras entidades só guardam o FK como `int`/`Guid`, sem navegação) —
necessária aqui porque o valor total depende de somar os itens, e o
Artigo VII (CQRS) já prevê handlers montando o DTO a partir da
entidade carregada com `Include`.

## `Common/StatusOrdemServico.cs`

Mesmo padrão de `Roles.cs`/`FormasPagamento.cs`:

```csharp
public static class StatusOrdemServico
{
    public const string Aberto = "Aberto";
    public const string EmAndamento = "Em Andamento";
    public const string AguardandoPeca = "Aguardando Peça";
    public const string Concluido = "Concluído";
    public const string Cancelado = "Cancelado";

    public static bool EhValido(string? valor) => valor is not null && Validos.Contains(valor);
}
```

## Tabelas / mapeamento

`ordens_servico` (snake_case, `builder.ToTable("ordens_servico")`):

| Propriedade | Coluna | Observação |
|---|---|---|
| Id | id | PK |
| ClienteId | cliente_id | FK → `clientes.id`, `DeleteBehavior.Restrict` (mesmo padrão de `Veiculo`) |
| VeiculoId | veiculo_id | FK → `veiculos.id`, `DeleteBehavior.Restrict` |
| TecnicoId | tecnico_id | FK → `usuarios.id`, nullable, `DeleteBehavior.SetNull` (se o usuário for removido, a OS não deve travar nem sumir — só perde o técnico designado) |
| Status | status | `MaxLength(20)` |
| DescricaoProblema | descricao_problema | `text` (sem limite curto — relato pode ser longo) |
| Diagnostico | diagnostico | `text`, nullable |
| Solucao | solucao | `text`, nullable |
| DataAbertura | data_abertura | |
| DataConclusao | data_conclusao | nullable |
| ValorMaoDeObra | valor_mao_de_obra | `decimal(10,2)` |
| Desconto | desconto | `decimal(10,2)` |
| AprovacaoClienteEm | aprovacao_cliente_em | nullable |
| AprovacaoClienteNome | aprovacao_cliente_nome | `MaxLength(150)`, nullable |

`itens_ordem_servico`:

| Propriedade | Coluna |
|---|---|
| Id | id |
| OrdemServicoId | ordem_servico_id (FK → `ordens_servico.id`, `DeleteBehavior.Cascade` — item não existe sem OS) |
| Descricao | descricao |
| Quantidade | quantidade (`decimal(10,2)`) |
| ValorUnitario | valor_unitario (`decimal(10,2)`) |

## DTOs

```csharp
public class OrdemServicoDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int VeiculoId { get; set; }
    public Guid? TecnicoId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string DescricaoProblema { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? Solucao { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataConclusao { get; set; }
    public decimal ValorMaoDeObra { get; set; }
    public decimal Desconto { get; set; }
    public DateTime? AprovacaoClienteEm { get; set; }
    public string? AprovacaoClienteNome { get; set; }
    public List<ItemOrdemServicoDto> Itens { get; set; } = new();
    public decimal ValorTotal { get; set; } // calculado no handler, nao gravado no banco
}

public class ItemOrdemServicoDto
{
    public int Id { get; set; }
    public int OrdemServicoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}
```

## Commands / Queries (CQRS) — mesmo padrão de Clientes/Veículos

| Ação | Tipo | Notas |
|---|---|---|
| Criar OS | `CreateOrdemServicoCommand` | valida `ClienteId`/`VeiculoId` existem e `VeiculoId` pertence ao `ClienteId`; se `TecnicoId` informado, valida que o usuário existe e tem `Role = "Mecânico"`; retorna `null`/resultado de erro (→ `400`) se não |
| Atualizar OS | `UpdateOrdemServicoCommand` | edita status/diagnóstico/solução/datas/valores; mesma validação de `TecnicoId` da criação; enum de resultado (`Success`/`NotFound`/`StatusInvalido`/`ClienteOuVeiculoInvalido`/`TecnicoInvalido`), mesmo padrão de `UpdateVeiculoResult` |
| Remover OS | `DeleteOrdemServicoCommand` | remove itens junto (cascade no banco resolve) |
| Adicionar item | `AddItemOrdemServicoCommand` | `OrdemServicoId` + descrição/quantidade/valor |
| Remover item | `RemoveItemOrdemServicoCommand` | por `Id` do item |
| Listar todas | `GetAllOrdensServicoQuery` | |
| Buscar por id | `GetOrdemServicoByIdQuery` | inclui itens + valor total |
| Listar por cliente | `GetOrdensServicoByClienteIdQuery` | mesmo padrão de `GetVeiculosByClienteId` |
| Listar por veículo | `GetOrdensServicoByVeiculoIdQuery` | histórico do veículo |

## Endpoints (`API/Controllers/OrdensServicoController.cs`)

| Método | Rota | Command/Query |
|---|---|---|
| GET | `/api/OrdensServico` | `GetAllOrdensServicoQuery` |
| GET | `/api/OrdensServico/{id}` | `GetOrdemServicoByIdQuery` |
| GET | `/api/OrdensServico/cliente/{clienteId}` | `GetOrdensServicoByClienteIdQuery` |
| GET | `/api/OrdensServico/veiculo/{veiculoId}` | `GetOrdensServicoByVeiculoIdQuery` |
| POST | `/api/OrdensServico` | `CreateOrdemServicoCommand` |
| PUT | `/api/OrdensServico/{id}` | `UpdateOrdemServicoCommand` |
| DELETE | `/api/OrdensServico/{id}` | `DeleteOrdemServicoCommand` |
| POST | `/api/OrdensServico/{id}/itens` | `AddItemOrdemServicoCommand` |
| DELETE | `/api/OrdensServico/itens/{itemId}` | `RemoveItemOrdemServicoCommand` |

`[Authorize]` na classe (mesmo padrão dos outros controllers de
negócio, já protegido desde a spec `autorizacao`).

## Repository

- `IOrdemServicoRepository`: `GetAllAsync`, `GetByIdAsync` (com
  `Include(o => o.Itens)`), `GetByClienteIdAsync`,
  `GetByVeiculoIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`
- Itens são manipulados através da própria `OrdemServico` carregada
  (`Itens.Add(...)`/`Itens.Remove(...)` + `SaveChanges`), sem
  repository próprio — evita duplicar a checagem "a OS existe?" em
  dois lugares.

## Riscos e decisões

- **Decisão:** `ValorTotal` é calculado no handler (soma dos itens +
  mão de obra − desconto), não é coluna no banco — evita duas fontes
  de verdade desincronizando. Ver "Fora de Escopo" na spec sobre
  snapshot no futuro.
- **Decisão:** `VeiculoId` obrigatório e validado contra
  `ClienteId` informado (o veículo tem que pertencer àquele cliente)
  — evita abrir OS de um veículo de outro cliente por engano.
- **Decisão (revisão 2026-09-13):** `TecnicoId`, quando informado, é
  validado contra `IUsuarioRepository` — usuário precisa existir **e**
  ter `Role == Roles.Mecanico` (`Common/Roles.cs`, mesma constante já
  usada em `perfis-usuario`); qualquer um dos dois motivos falhando
  retorna `TecnicoInvalido` → controller mapeia pra `400` com mensagem
  distinguindo "técnico não encontrado" de "usuário não é mecânico".
- **Decisão (revisão 2026-09-13):** `PrevisaoTermino` removida por
  completo (entidade, configuração EF, DTO, commands) — migration nova
  (`RemovePrevisaoTermino...`) derruba a coluna `previsao_termino`.
  Sem dado a migrar/preservar (feature `calendario`, que passa a cobrir
  agendamento/data prevista, é uma tabela própria — ver
  `specs/calendario/`).
- **Decisão:** itens (peças) têm seus próprios endpoints
  (`POST .../itens`, `DELETE .../itens/{id}`) em vez de reescrever a
  lista inteira no `PUT` da OS — evita o cliente HTTP ter que reenviar
  todos os itens pra adicionar um novo.

## Estratégia de verificação

- `dotnet build` sem erros.
- Migration aplicada localmente.
- Testar via curl (token válido):
  - Criar OS com `ClienteId`/`VeiculoId` válidos → `201`, `status`
    "Aberto", `dataAbertura` preenchida.
  - Criar OS com `VeiculoId` de outro cliente → erro claro (não
    `500`).
  - Adicionar 2 itens → `GET` da OS mostra os itens e `valorTotal`
    correto (mão de obra + itens − desconto).
  - Atualizar status pra `"Concluído"` → reflete no `GET`.
  - Atualizar com status inválido (`"Pronto"`) → erro claro.
  - Remover a OS → itens somem junto; `GET` por id retorna `404`.
  - Listar por cliente e por veículo → filtra certo.
  - Registros de teste removidos depois.
