# Design — Calendário (Agendamentos)

## Entidade (`Core/Domain/Entities/Agendamento.cs`)

```csharp
public class Agendamento
{
    public int Id { get; set; }
    public Guid TecnicoId { get; set; }
    public int? ClienteId { get; set; }
    public int? VeiculoId { get; set; }
    public int? OrdemServicoId { get; set; }
    public DateTime DataHoraInicio { get; set; }
    public DateTime? DataHoraFim { get; set; }
    public string? Observacao { get; set; }
    public string Status { get; set; } = StatusAgendamento.Agendado;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
```

## `Common/StatusAgendamento.cs`

Mesmo padrão de `StatusOrdemServico.cs`:

```csharp
public static class StatusAgendamento
{
    public const string Agendado = "Agendado";
    public const string Concluido = "Concluído";
    public const string Cancelado = "Cancelado";

    public static bool EhValido(string? valor) => valor is not null && Validos.Contains(valor);
}
```

## `Common/SlotAgendamento.cs` — granularidade de 3 minutos

```csharp
public static class SlotAgendamento
{
    public static readonly TimeSpan Duracao = TimeSpan.FromMinutes(3);

    public static DateTime Truncar(DateTime dataHora)
    {
        var ticksSlot = Duracao.Ticks;
        return new DateTime(dataHora.Ticks - (dataHora.Ticks % ticksSlot), dataHora.Kind);
    }
}
```

Usado tanto na escrita (`Create`/`UpdateAgendamentoHandler` truncam
`DataHoraInicio` antes de gravar) quanto na leitura
(`GetConflitoAgendamentoQuery` trunca o parâmetro recebido antes de
comparar) — garante que os dois lados sempre comparam o mesmo grão de
tempo, independente de o cliente mandar um segundo/milissegundo
qualquer.

## Tabela / mapeamento

`agendamentos` (`AgendamentoConfiguration.cs`, novo):

| Propriedade | Coluna | Observação |
|---|---|---|
| Id | id | PK |
| TecnicoId | tecnico_id | FK → `usuarios.id`, `DeleteBehavior.Restrict` (agendamento não pode ficar órfão de técnico — diferente de `OrdemServico.TecnicoId`, que é opcional) |
| ClienteId | cliente_id | FK → `clientes.id`, nullable, `DeleteBehavior.SetNull` |
| VeiculoId | veiculo_id | FK → `veiculos.id`, nullable, `DeleteBehavior.SetNull` |
| OrdemServicoId | ordem_servico_id | FK → `ordens_servico.id`, nullable, `DeleteBehavior.SetNull` (remover a OS não deve apagar o agendamento, só soltar o vínculo) |
| DataHoraInicio | data_hora_inicio | |
| DataHoraFim | data_hora_fim | nullable |
| Observacao | observacao | `text`, nullable |
| Status | status | `MaxLength(20)` |
| DataCadastro | data_cadastro | |

Índice composto `(tecnico_id, data_hora_inicio)` — a checagem de
conflito e a listagem por técnico/dia são as consultas mais frequentes
desta tabela.

## DTO (`AgendamentoDto.cs`)

```csharp
public class AgendamentoDto
{
    public int Id { get; set; }
    public Guid TecnicoId { get; set; }
    public int? ClienteId { get; set; }
    public int? VeiculoId { get; set; }
    public int? OrdemServicoId { get; set; }
    public DateTime DataHoraInicio { get; set; }
    public DateTime? DataHoraFim { get; set; }
    public string? Observacao { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
```

## Repository (`IAgendamentoRepository`)

```csharp
public interface IAgendamentoRepository
{
    Task<List<Agendamento>> GetAllAsync();
    Task<Agendamento?> GetByIdAsync(int id);
    Task<List<Agendamento>> GetByTecnicoIdAsync(Guid tecnicoId, DateTime? data = null);
    Task<Agendamento?> GetConflitoAsync(Guid tecnicoId, DateTime slotInicio, int? ignorarId = null);
    Task AddAsync(Agendamento agendamento);
    Task UpdateAsync(Agendamento agendamento);
    Task DeleteAsync(Agendamento agendamento);
}
```

`GetConflitoAsync` filtra `Status == StatusAgendamento.Agendado` e
`TecnicoId`/`DataHoraInicio` (já truncado) iguais; `ignorarId` existe
pra `UpdateAgendamentoHandler` não conflitar consigo mesmo ao apenas
editar a observação de um agendamento existente sem mudar o horário.

## Commands / Queries (CQRS)

| Ação | Tipo | Notas |
|---|---|---|
| Criar agendamento | `CreateAgendamentoCommand` | valida técnico existe e é Mecânico; trunca slot; rejeita se `GetConflitoAsync` achar algo (`Conflito`, ver enum abaixo) |
| Atualizar agendamento | `UpdateAgendamentoCommand` | mesma validação de técnico/slot/conflito (ignorando o próprio id); enum de resultado (`Success`/`NotFound`/`TecnicoInvalido`/`Conflito`) |
| Remover agendamento | `DeleteAgendamentoCommand` | hard delete |
| Listar todos | `GetAllAgendamentosQuery` | |
| Buscar por id | `GetAgendamentoByIdQuery` | |
| Listar por técnico | `GetAgendamentosByTecnicoIdQuery` | parâmro opcional de dia, pra ver a agenda de um técnico num dia específico |
| Checar conflito | `GetConflitoAgendamentoQuery` | `TecnicoId` + `DataHora` → `AgendamentoDto?` |

```csharp
public enum CriarOuAtualizarAgendamentoResultado
{
    Sucesso,
    NotFound,          // só em Update
    TecnicoInvalido,   // usuário não existe ou não é Mecânico
    Conflito           // já existe outro Agendado no mesmo slot
}

public class CriarOuAtualizarAgendamentoResult
{
    public CriarOuAtualizarAgendamentoResultado Resultado { get; set; }
    public AgendamentoDto? Agendamento { get; set; }        // preenchido em Sucesso
    public AgendamentoDto? AgendamentoConflitante { get; set; } // preenchido em Conflito
}
```

## Endpoints (`API/Controllers/AgendamentosController.cs`, novo)

| Método | Rota | Command/Query |
|---|---|---|
| GET | `/api/Agendamentos` | `GetAllAgendamentosQuery` |
| GET | `/api/Agendamentos/{id}` | `GetAgendamentoByIdQuery` |
| GET | `/api/Agendamentos/tecnico/{tecnicoId}?data=yyyy-MM-dd` | `GetAgendamentosByTecnicoIdQuery` |
| GET | `/api/Agendamentos/conflito?tecnicoId=&dataHora=` | `GetConflitoAgendamentoQuery` → `200` com corpo ou `204` |
| POST | `/api/Agendamentos` | `CreateAgendamentoCommand` → `201`/`400` (técnico inválido)/`409` (conflito) |
| PUT | `/api/Agendamentos/{id}` | `UpdateAgendamentoCommand` → `204`/`404`/`400`/`409` |
| DELETE | `/api/Agendamentos/{id}` | `DeleteAgendamentoCommand` |

`[Authorize]` na classe, sem restrição de role (mesmo padrão dos
outros controllers de negócio).

### Controller — mapeamento do resultado de Create/Update

```csharp
return resultado.Resultado switch
{
    CriarOuAtualizarAgendamentoResultado.Sucesso => Ok(resultado.Agendamento), // ou Created no POST
    CriarOuAtualizarAgendamentoResultado.NotFound => NotFound(),
    CriarOuAtualizarAgendamentoResultado.TecnicoInvalido =>
        BadRequest(new { message = "Técnico inválido: usuário não encontrado ou não é Mecânico." }),
    CriarOuAtualizarAgendamentoResultado.Conflito =>
        Conflict(new { message = "Técnico já tem agendamento neste horário.", agendamento = resultado.AgendamentoConflitante }),
    _ => StatusCode(500)
};
```

## Riscos e decisões

- **Decisão:** a checagem de conflito compara slot exato
  (`TecnicoId` + `DataHoraInicio` truncada), não overlap de intervalo
  — decisão registrada na spec (Suposições), mais simples de
  implementar e alinhada ao pedido literal ("mesmo dia e mesma hora").
- **Decisão:** `Create`/`UpdateAgendamentoHandler` rejeitam conflito no
  servidor (`409`) além da checagem prévia que o frontend faz via
  `/conflito` — dupla camada, mesmo padrão de "servidor é a fonte de
  verdade" já usado em Estoque (o cliente não pode confiar só na
  própria checagem prévia).
- **Risco aceito:** sem transação/lock dedicado entre a leitura de
  conflito e a escrita em `Create`/`UpdateAgendamentoHandler` — duas
  requisições simultâneas pro mesmo slot podem, em teoria, passar
  pela checagem antes de uma delas gravar; aceitável pro volume de uma
  oficina (mesmo risco já aceito em Estoque).
- **Decisão:** `TecnicoId` obrigatório em `Agendamento` (diferente de
  `OrdemServico.TecnicoId`, que é opcional) — um agendamento sem
  técnico não faz sentido de existir.
- **Decisão:** granularidade de 3 minutos implementada só por
  truncamento em C#, sem constraint de banco — mesmo nível de
  validação "aceitável pro estágio atual" do resto do projeto (sem
  FluentValidation ainda, constitution seção 6).

## Estratégia de verificação

- `dotnet build` sem erros.
- Migration aplicada localmente.
- Testar via curl (token válido):
  - Criar agendamento com técnico Mecânico válido → `201`.
  - Criar segundo agendamento pro mesmo técnico, mesmo slot (mesmo
    minuto, ou minuto dentro dos mesmos 3 min) → `409`, corpo com o
    agendamento existente.
  - Criar agendamento pro mesmo técnico em um slot diferente (3 minutos
    depois) → `201`, sem conflito.
  - Criar com `tecnicoId` de um usuário não-Mecânico → `400`.
  - `GET /conflito` com slot ocupado → `200` com o agendamento; slot
    livre → `204`.
  - Editar um agendamento (mudar observação, sem mudar horário) → não
    conflita consigo mesmo (`ignorarId`).
  - Editar mudando o horário pra um slot já ocupado por outro
    agendamento → `409`.
  - Remover agendamento → `204`; `GET` por id depois → `404`.
  - Listar por técnico filtrando por dia → retorna só os do dia.
  - Registros de teste removidos depois.
