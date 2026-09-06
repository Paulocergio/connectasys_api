# Design — Conclusão de OS gera Conta a Receber

> Ver nota no topo do `spec.md`: o gatilho mudou de um endpoint
> dedicado de "aprovar" para a transição de `Status` para
> `"Concluído"` dentro do `UpdateOrdemServicoCommand` já existente.

## Entidade (`Core/Domain/Entities/ContaReceber.cs`) — campo novo

```csharp
public class ContaReceber
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataRecebimento { get; set; }
    public string? FormaPagamento { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public int? OrdemServicoId { get; set; } // novo — null = lançamento manual
}
```

## Mapeamento (`ContaReceberConfiguration.cs`) — acréscimo

```csharp
builder.Property(c => c.OrdemServicoId).HasColumnName("ordem_servico_id");

builder.HasOne<OrdemServico>()
    .WithMany()
    .HasForeignKey(c => c.OrdemServicoId)
    .OnDelete(DeleteBehavior.Cascade);
```

`Cascade` (não `SetNull`): excluir a OS remove junto a conta a receber
gerada por ela — decisão explícita da spec, diferente do padrão usado
em outros FKs opcionais do sistema (ex.: `TecnicoId` em `OrdemServico`
usa `SetNull`).

Duas migrations:
1. `AddOrdemServicoIdToContasReceber` — coluna + FK inicial (criada
   com `SetNull` na primeira rodada de implementação).
2. `ContaReceberCascadeAoDeletarOS` — corrige o `OnDelete` pra
   `Cascade`, refletindo a decisão final.

## DTO (`ContaReceberDto.cs`) — campo novo

Igual à primeira rodada — `OrdemServicoId` mapeado nos 4 pontos que
constroem `ContaReceberDto` (`GetAllContasReceberHandler`,
`GetContaReceberByIdHandler`, `GetContasReceberByClienteIdHandler`,
`CreateContaReceberHandler`, este último sempre `null`).

## `IContaReceberRepository` — método novo

```csharp
Task<ContaReceber?> GetByOrdemServicoIdAsync(int ordemServicoId);
```

```csharp
public async Task<ContaReceber?> GetByOrdemServicoIdAsync(int ordemServicoId) =>
    await _context.ContasReceber.FirstOrDefaultAsync(c => c.OrdemServicoId == ordemServicoId);
```

Usado tanto pra não duplicar a conta (se já existe uma, não cria outra)
quanto pra sincronizar o valor quando a OS muda depois de já ter
gerado a conta.

## `IOrdemServicoRepository` — método novo (transacional)

```csharp
Task SalvarComContaReceberAsync(
    OrdemServico ordemServico,
    ContaReceber? contaReceberNova,
    ContaReceber? contaReceberParaAtualizar,
    ContaReceber? contaReceberParaRemover);
```

```csharp
public async Task SalvarComContaReceberAsync(
    OrdemServico ordemServico,
    ContaReceber? contaReceberNova,
    ContaReceber? contaReceberParaAtualizar,
    ContaReceber? contaReceberParaRemover)
{
    await using var transaction = await _context.Database.BeginTransactionAsync();
    _context.OrdensServico.Update(ordemServico);
    if (contaReceberNova is not null)
        _context.ContasReceber.Add(contaReceberNova);
    if (contaReceberParaAtualizar is not null)
        _context.ContasReceber.Update(contaReceberParaAtualizar);
    if (contaReceberParaRemover is not null)
        _context.ContasReceber.Remove(contaReceberParaRemover);
    await _context.SaveChangesAsync();
    await transaction.CommitAsync();
}
```

Os três parâmetros são mutuamente exclusivos na prática: ou a OS ainda
não tinha conta e uma é criada (`contaReceberNova`), ou já tinha e o
valor é atualizado (`contaReceberParaAtualizar`), ou a OS foi cancelada
e a conta existente é removida (`contaReceberParaRemover`).

## `UpdateOrdemServicoHandler` — lógica

```
1. ordemServico = repository.GetByIdAsync(Id)
2. ...validações existentes (cliente, veículo, técnico, status)...
3. statusAnterior = ordemServico.Status   // antes de sobrescrever
4. aplica todos os campos do request na entidade, incluindo
   AprovacaoClienteEm/AprovacaoClienteNome (voltaram a ser editáveis
   aqui, sem relação com a conta a receber)
5. acabouDeConcluir = statusAnterior != "Concluído"
                       && ordemServico.Status == "Concluído"
6. contaExistente = contaReceberRepository.GetByOrdemServicoIdAsync(Id)
7. valorTotal = ValorMaoDeObra + soma(itens) - Desconto  // sempre, não só ao concluir
8. ContaReceber? contaNova = null, contaParaAtualizar = null, contaParaRemover = null
   se ordemServico.Status == "Cancelado":
     contaParaRemover = contaExistente   // null-safe, sem efeito se não houver conta
   senão se contaExistente != null:
     contaExistente.Valor = valorTotal   // mantém sincronizado
     contaParaAtualizar = contaExistente
   senão se acabouDeConcluir e valorTotal > 0:
     dataBase = ordemServico.DataConclusao ?? DateTime.UtcNow
     contaNova = new ContaReceber { ..., DataVencimento = dataBase.AddDays(30) }
9. repository.SalvarComContaReceberAsync(ordemServico, contaNova, contaParaAtualizar, contaParaRemover)
10. retorna Success
```

`UpdateOrdemServicoCommand` recupera os dois campos
`AprovacaoClienteEm`/`AprovacaoClienteNome` (revertendo a remoção da
primeira rodada) — o `PUT` genérico volta a aceitá-los como antes.

## Sincronização de valor via itens (`AddItem`/`RemoveItem`)

Adicionar ou remover um item (peça) também muda o valor total da OS,
então os dois handlers precisam da mesma sincronização:

- `AddItemOrdemServicoHandler`: depois de salvar o item novo (que já
  é persistido pelo `AddItemAsync` existente), recarrega a OS
  (`repository.GetByIdAsync`) pra obter a lista de itens **já
  incluindo o novo** (evita contar o item duas vezes — o EF Core faz
  fixup automático da coleção `Itens` em memória quando o item é
  adicionado no mesmo `DbContext`, então somar `Itens` de antes +
  o item novo separadamente duplicava o valor; a recarga resolve
  isso de forma simples e explícita) e, se existir conta vinculada,
  atualiza `Valor` com `IContaReceberRepository.UpdateAsync`.
- `RemoveItemOrdemServicoHandler`: depois de remover o item, recarrega
  a OS (sem o item removido) e faz o mesmo ajuste, se houver conta
  vinculada.

Essas duas sincronizações **não** passam pelo método transacional
`SalvarComContaReceberAsync` — são um `UpdateAsync` simples e
independente do save do item, aceitável porque não criam registro
novo (só ajustam um valor numérico existente), diferente da criação
inicial da conta.

## Endpoint dedicado removido

`POST /api/OrdensServico/{id}/aprovar`, `AprovarOrdemServicoCommand` e
`AprovarOrdemServicoHandler` da primeira rodada foram **removidos** —
o gatilho passou a ser o `PUT` genérico, então não fazem mais sentido.

## Exclusão de OS

`DeleteOrdemServicoHandler`/`DeleteOrdemServicoCommand` não mudam —
a remoção em cascata da conta a receber é resolvida pelo `ON DELETE
CASCADE` no banco (constraint da FK), sem precisar de lógica de
aplicação nova.

## Riscos e decisões

- **Decisão:** verificar duplicidade por "já existe uma
  `ContaReceber` com esse `OrdemServicoId`" em vez de guardar um flag
  separado — mais simples, sem campo extra, e resiliente a qualquer
  jeito de a OS voltar a ficar "Concluído" depois.
- **Decisão:** `DataVencimento` usa `DataConclusao` da OS como base
  (não a data/hora do save) — se o usuário registrar uma
  `DataConclusao` retroativa, o vencimento reflete isso; se não
  preencher `DataConclusao`, cai no `DateTime.UtcNow`.
- **Risco aceito:** se o usuário voltar o status de "Concluído" pra
  outro valor e depois pra "Concluído" de novo, nenhuma conta nova é
  gerada (já existe uma) — mesmo que a primeira já tenha sido editada/
  paga. Documentado como Fora de Escopo.

## Estratégia de verificação

- `dotnet build` sem erros.
- Migrations aplicadas localmente.
- Testado via curl (token válido):
  - Criar OS com mão de obra + item (total 200) → `PUT` mudando
    `status` pra `"Concluído"` com `dataConclusao` → `204`; nova
    `ContaReceber` aparece com `ordemServicoId` correto, `valor`
    correto, `dataVencimento` = `dataConclusao` + 30 dias.
  - Salvar a mesma OS de novo com `status = "Concluído"` → `204`, sem
    conta duplicada.
  - Criar OS com valor zero → concluir → `204`, nenhuma conta gerada.
  - Excluir a OS que gerou conta → a conta some junto (confirmado via
    `GET /api/ContasReceber`, contagem antes/depois).
  - Registros de teste removidos depois.
