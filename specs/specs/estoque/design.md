# Design — Estoque

> **Revisão 2** (pedido do usuário após a primeira entrega): o modelo
> de `Estoque` mudou — `Descricao` virou `Nome` (obrigatório) +
> `Descricao` (opcional); `ValorUnitario` virou `PrecoVenda`; novo
> `PrecoCompra`; novo `EstoqueMinimo`. Como a Fase 1 (migration) ainda
> não tinha sido implantada em produção, a migration original
> (`AddEstoque`) foi refeita do zero em vez de empilhar uma segunda —
> ver `tasks.md`.
>
> **Revisão 3 (2026-09-13):** `EstoqueMinimo` removido — coluna cai
> numa migration nova (a Fase 1/2 já está em produção desta vez, então
> não dá pra refazer a migration original do zero como na Revisão 2;
> tem que ser uma migration incremental de remoção). `Margem` continua
> sem propriedade própria — só passa a virar duas contas no frontend
> em vez de uma (Markup e Venda), sem qualquer mudança de schema.

## Entidade (`Core/Domain/Entities/Estoque.cs`)

```csharp
public class Estoque
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Quantidade { get; set; }
    public decimal PrecoCompra { get; set; }
    public decimal PrecoVenda { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
```

`EstoqueMinimo` removida (revisão 2026-09-13). `Margem` não existe
como propriedade — é sempre calculada no frontend, nas duas variantes
descritas no `spec.md` (Markup e Venda), na hora de exibir.

## `ItemOrdemServico` — campo novo

```csharp
public class ItemOrdemServico
{
    public int Id { get; set; }
    public int OrdemServicoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public int? EstoqueId { get; set; } // novo — null = item avulso, sem vínculo
}
```

## Mapeamento

`estoque` (`EstoqueConfiguration.cs`, novo):

| Propriedade | Coluna | Observação |
|---|---|---|
| Id | id | PK |
| Descricao | descricao | `MaxLength(200)` |
| Quantidade | quantidade | `numeric(10,2)` |
| ValorUnitario | valor_unitario | `numeric(10,2)` |
| DataCadastro | data_cadastro | |

`itens_ordem_servico` (`ItemOrdemServicoConfiguration.cs`, acréscimo):

```csharp
builder.Property(i => i.EstoqueId).HasColumnName("estoque_id");

builder.HasOne<Estoque>()
    .WithMany()
    .HasForeignKey(i => i.EstoqueId)
    .OnDelete(DeleteBehavior.SetNull);
```

`SetNull`: remover uma peça do estoque não pode travar nem apagar
itens de OS que já a usaram — só solta a referência (item continua
existindo, com descrição/valor congelados no que foram gravados na
hora, `EstoqueId` passa a `null`). Ajuste em relação ao texto do
`spec.md` (que falava em "apontar pra um registro que não existe
mais") — na prática o efeito visível é o mesmo (item preservado, sem
vínculo utilizável), só que via `SetNull` em vez de deixar uma FK
"solta".

Migration: `AddEstoque` (nova tabela) +
`AddEstoqueIdToItensOrdemServico` (coluna + FK), ou uma única
migration cobrindo as duas mudanças — decisão de implementação, não
muda o resultado final.

## DTO (`EstoqueDto.cs`)

```csharp
public class EstoqueDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public DateTime DataCadastro { get; set; }
}
```

`ItemOrdemServicoDto` ganha `EstoqueId` (`int?`).

## Repository (`IEstoqueRepository` + `EstoqueRepository`)

Mesmo padrão de `IContaPagarRepository`:

```csharp
public interface IEstoqueRepository
{
    Task<List<Estoque>> GetAllAsync();
    Task<Estoque?> GetByIdAsync(int id);
    Task AddAsync(Estoque estoque);
    Task UpdateAsync(Estoque estoque);
    Task DeleteAsync(Estoque estoque);
}
```

## Commands / Queries (CQRS)

| Ação | Tipo | Notas |
|---|---|---|
| Criar peça | `CreateEstoqueCommand` | sem validação de negativo (ver spec, Fora de Escopo) |
| Atualizar peça | `UpdateEstoqueCommand` | enum `UpdateEstoqueResult` (`Success`/`NotFound`) |
| Remover peça | `DeleteEstoqueCommand` | `bool` |
| Listar todas | `GetAllEstoqueQuery` | |
| Buscar por id | `GetEstoqueByIdQuery` | |

## Endpoints (`API/Controllers/EstoqueController.cs`, novo)

| Método | Rota | Command/Query |
|---|---|---|
| GET | `/api/Estoque` | `GetAllEstoqueQuery` |
| GET | `/api/Estoque/{id}` | `GetEstoqueByIdQuery` |
| POST | `/api/Estoque` | `CreateEstoqueCommand` |
| PUT | `/api/Estoque/{id}` | `UpdateEstoqueCommand` |
| DELETE | `/api/Estoque/{id}` | `DeleteEstoqueCommand` |

`[Authorize]` na classe, sem restrição de role (mesmo padrão da
maioria dos controllers de negócio).

## `AddItemOrdemServicoCommand`/`Handler` — mudança de contrato

O retorno deixa de ser `ItemOrdemServicoDto?` (null só significava "OS
não encontrada") — agora precisa distinguir 3 motivos de falha:

```csharp
public enum AddItemOrdemServicoResultado
{
    Sucesso,
    OrdemServicoNaoEncontrada,
    EstoqueNaoEncontrado,
    EstoqueInsuficiente
}

public class AddItemOrdemServicoResult
{
    public AddItemOrdemServicoResultado Resultado { get; set; }
    public ItemOrdemServicoDto? Item { get; set; }
}

public class AddItemOrdemServicoCommand : IRequest<AddItemOrdemServicoResult>
{
    public int OrdemServicoId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public int? EstoqueId { get; set; } // novo
}
```

### Handler

```
1. ordemServico = repository.GetByIdAsync(OrdemServicoId)
   → null: retorna OrdemServicoNaoEncontrada
2. Estoque? estoque = null
   se request.EstoqueId != null:
     estoque = estoqueRepository.GetByIdAsync(EstoqueId)
     → null: retorna EstoqueNaoEncontrado
     se estoque.Quantidade < request.Quantidade: retorna EstoqueInsuficiente
     descricao = estoque.Descricao      // servidor é a fonte de verdade
     valorUnitario = estoque.ValorUnitario
   senão:
     descricao = request.Descricao
     valorUnitario = request.ValorUnitario
3. item = new ItemOrdemServico { ..., EstoqueId = request.EstoqueId }
4. repository.AddItemAsync(item)
5. se estoque != null:
     estoque.Quantidade -= request.Quantidade
     estoqueRepository.UpdateAsync(estoque)
6. ...sincronização de ContaReceber já existente (Fase 5 da feature
   anterior), sem mudança...
7. retorna Sucesso + o item criado
```

Passos 4 e 5 não são atômicos entre si (mesmo padrão pragmático já
aceito na sincronização de `ContaReceber` — ver "Riscos e decisões").

## `RemoveItemOrdemServicoHandler` — devolve quantidade ao estoque

```
1. item = repository.GetItemByIdAsync(ItemId)
   → null: retorna false
2. guarda ordemServicoId, estoqueId e quantidade do item antes de remover
3. repository.RemoveItemAsync(item)
4. se estoqueId != null:
     estoque = estoqueRepository.GetByIdAsync(estoqueId)
     se estoque != null: estoque.Quantidade += quantidade; UpdateAsync
5. ...sincronização de ContaReceber já existente, sem mudança...
6. retorna true
```

## `DeleteOrdemServicoHandler` — devolve estoque de todos os itens

```
1. ordemServico = repository.GetByIdAsync(Id)  // já inclui Itens
   → null: retorna false
2. para cada item em ordemServico.Itens onde EstoqueId != null:
     estoque = estoqueRepository.GetByIdAsync(item.EstoqueId)
     se estoque != null: estoque.Quantidade += item.Quantidade; UpdateAsync
3. repository.DeleteAsync(ordemServico)  // cascade apaga os itens no banco
4. retorna true
```

## Endpoint `AddItem` — controller atualizado

```csharp
[HttpPost("{id}/itens")]
public async Task<IActionResult> AddItem(int id, AddItemOrdemServicoCommand command)
{
    if (id != command.OrdemServicoId) return BadRequest();
    var resultado = await _mediator.Send(command);
    return resultado.Resultado switch
    {
        AddItemOrdemServicoResultado.Sucesso => Ok(resultado.Item),
        AddItemOrdemServicoResultado.OrdemServicoNaoEncontrada =>
            NotFound("Ordem de serviço não encontrada."),
        AddItemOrdemServicoResultado.EstoqueNaoEncontrado =>
            BadRequest(new { message = "Peça de estoque não encontrada." }),
        AddItemOrdemServicoResultado.EstoqueInsuficiente =>
            BadRequest(new { message = "Quantidade insuficiente em estoque." }),
        _ => StatusCode(500)
    };
}
```

## Riscos e decisões

- **Decisão:** servidor sempre recalcula `Descricao`/`ValorUnitario` a
  partir do `Estoque` quando `EstoqueId` é informado — o cliente pode
  mandar qualquer coisa nesses dois campos que será ignorado, evita
  manipulação de preço pelo front.
- **Decisão:** débito/devolução de estoque não usa uma transação
  dedicada com o item — mesmo nível de consistência "aceitável pro
  estágio atual" já usado na sincronização de `ContaReceber` (Fase 5
  da feature `aprovacao-os-conta-receber`), documentado aqui em vez de
  generalizado com uma abstração de Unit of Work nova.
- **Risco aceito:** duas requisições simultâneas adicionando itens da
  mesma peça de estoque podem gerar uma condição de corrida (ambas
  leem a mesma `Quantidade` disponível antes de uma delas debitar) —
  aceitável pro volume de uso atual de uma oficina; resolver
  exigiria lock otimista/transação com nível de isolamento mais alto,
  fora de escopo agora.

## Estratégia de verificação

- `dotnet build` sem erros.
- Migrations aplicadas localmente.
- Testado via curl (token válido):
  - Criar peça no estoque (quantidade 10, valor 25) → `201`.
  - Adicionar item na OS com `estoqueId` dessa peça, quantidade 3 →
    `200`, item criado com descrição/valor vindos do estoque; `GET`
    do estoque mostra quantidade 7.
  - Adicionar item pedindo quantidade 100 (maior que o disponível) →
    `400`, estoque inalterado.
  - Remover o item criado → estoque volta pra 10.
  - Criar OS, adicionar item vinculado, excluir a OS inteira → estoque
    volta ao valor original.
  - Adicionar item sem `estoqueId` (fluxo antigo) → continua
    funcionando, sem tocar em nenhum estoque.
  - Registros de teste removidos depois.
