# Design — Forma de Pagamento (Contas a Pagar e Contas a Receber)

## Conjunto fechado de valores (`Core/Application/Common/FormasPagamento.cs`)

Arquivo novo, mesmo espírito de `StatusConta.cs` (constantes + helper),
na mesma pasta:

```csharp
public static class FormasPagamento
{
    public const string Cartao = "Cartão";
    public const string Pix = "Pix";
    public const string Boleto = "Boleto";
    public const string Dinheiro = "Dinheiro";

    private static readonly HashSet<string> Validas = new()
    {
        Cartao, Pix, Boleto, Dinheiro
    };

    public static bool EhValida(string? valor) => valor is not null && Validas.Contains(valor);
}
```

Nome `FormasPagamento` (plural) — não `FormaPagamento` — de propósito,
pra não colidir visualmente com a propriedade `FormaPagamento` que as
duas entidades ganham (evita `FormaPagamento.EhValida(...)` ao lado de
`contaPagar.FormaPagamento` no mesmo método).

## Entidades

`ContaPagar` e `ContaReceber` ganham a mesma propriedade:

```csharp
public string? FormaPagamento { get; set; }
```

`null` enquanto a conta não foi paga/recebida — mesmo espírito de
`DataPagamento`/`DataRecebimento`.

## Configuration (mapeamento)

Em `ContaPagarConfiguration.cs` e `ContaReceberConfiguration.cs`:

```csharp
builder.Property(c => c.FormaPagamento).HasColumnName("forma_pagamento").HasMaxLength(20);
```

Coluna nullable (comportamento padrão do EF pra `string?`).

## Migration

Uma migration cobrindo as duas tabelas:
`dotnet ef migrations add AddFormaPagamentoContasPagarEContasReceber`.

## DTOs

`ContaPagarDto` e `ContaReceberDto` ganham:

```csharp
public string? FormaPagamento { get; set; }
```

## Commands — regra de validação (o núcleo desta feature)

Só os comandos de **Update** mudam (Create continua sem
`DataPagamento`/`DataRecebimento`/`FormaPagamento`, sem alteração).

`UpdateContaPagarCommand` e `UpdateContaReceberCommand` ganham:

```csharp
public string? FormaPagamento { get; set; }
```

`UpdateContaPagarCommand` hoje é `IRequest<bool>` — muda pra um enum de
resultado, no mesmo padrão já usado em `UpdateContaReceberCommand`:

```csharp
public enum UpdateContaPagarResult
{
    Success,
    ContaNotFound,
    FormaPagamentoInvalida
}
```

`UpdateContaReceberResult` (já existe) ganha mais um caso:

```csharp
public enum UpdateContaReceberResult
{
    Success,
    ContaNotFound,
    ClienteInvalido,
    FormaPagamentoInvalida
}
```

### Regra, igual nos dois handlers (`UpdateContaPagarHandler` /
`UpdateContaReceberHandler`)

```csharp
if (request.DataPagamento.HasValue && !FormasPagamento.EhValida(request.FormaPagamento))
    return UpdateContaPagarResult.FormaPagamentoInvalida; // idem ContaReceber, com DataRecebimento

contaPagar.DataPagamento = request.DataPagamento;
contaPagar.FormaPagamento = request.DataPagamento.HasValue ? request.FormaPagamento : null;
```

- Sem `DataPagamento`/`DataRecebimento` na requisição → `FormaPagamento`
  é ignorado e gravado como `null`, mesmo que o cliente tenha mandado
  algo (RF-04 da spec do hub / critério de aceite da spec deste repo).
- Com `DataPagamento`/`DataRecebimento` e `FormaPagamento` ausente ou
  fora do conjunto fechado → `FormaPagamentoInvalida`, sem gravar nada
  (`SaveChanges` não é chamado).
- `ContaReceberHandler` mantém a ordem de validação já existente:
  primeiro busca a conta (`ContaNotFound`), depois valida `ClienteId`
  (`ClienteInvalido`), e só então a forma de pagamento
  (`FormaPagamentoInvalida`) — mesma ordem "existência antes de regra
  de negócio" que já existia pro cliente.

## Controllers

`ContasPagarController.Update` passa a espelhar o switch já usado em
`ContasReceberController.Update`:

```csharp
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, UpdateContaPagarCommand command)
{
    if (id != command.Id) return BadRequest();

    var result = await _mediator.Send(command);
    return result switch
    {
        UpdateContaPagarResult.Success => NoContent(),
        UpdateContaPagarResult.ContaNotFound => NotFound(),
        UpdateContaPagarResult.FormaPagamentoInvalida =>
            BadRequest("FormaPagamento é obrigatória e deve ser Cartão, Pix, Boleto ou Dinheiro ao informar DataPagamento."),
        _ => StatusCode(500)
    };
}
```

`ContasReceberController.Update` ganha só o novo `case` no switch já
existente, mesma mensagem trocando "DataPagamento" por
"DataRecebimento".

## Repositórios

Sem mudança — `FormaPagamento` é só mais uma propriedade da entidade já
mapeada; `AddAsync`/`UpdateAsync` genéricos continuam funcionando.

## Riscos e Decisões

- **Decisão:** validação por código (não FluentValidation) — projeto
  ainda não tem FluentValidation (constitution, seção 6); segue o
  padrão já usado pra `ClienteId` em `ContaReceber`.
- **Decisão:** `FormaPagamento` some (`null`) se a conta for
  "despaga" numa edição futura (enviar `DataPagamento: null` numa
  conta já paga) — mesmo espírito do comentário já existente em
  `UpdateContaPagarCommand`/design de Contas a Pagar sobre permitir
  desmarcar um pagamento lançado por engano.
- **Risco:** mudar `UpdateContaPagarCommand`/`Controller` de `bool`
  pra enum é uma mudança de contrato de retorno da API só na forma
  como erros são reportados (`400` novo em vez de nunca acontecer);
  não muda `200`/`204`/`404` já existentes, então não quebra o hub
  (`_app.contas-a-pagar.tsx` já trata qualquer erro via `onError` do
  `useMutation` + toast).

## Estratégia de Verificação

- `dotnet build` sem erros.
- `dotnet ef database update` aplica a migration sem erro.
- Via `curl`, pra `ContasPagar` e `ContasReceber`:
  - Atualizar informando `dataPagamento`/`dataRecebimento` +
    `formaPagamento` válida → `204`; `GET` confirma `status`="Paga" e
    `formaPagamento` gravada.
  - Atualizar informando `dataPagamento`/`dataRecebimento` sem
    `formaPagamento` → `400`.
  - Atualizar informando `dataPagamento`/`dataRecebimento` com
    `formaPagamento` fora do conjunto (ex: `"Cheque"`) → `400`.
  - Atualizar sem `dataPagamento`/`dataRecebimento` (conta continua
    pendente), com ou sem `formaPagamento` no payload → `204`,
    `formaPagamento` gravado como `null`.
  - Registro de teste criado e removido depois.
