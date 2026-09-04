# Design — Perfis de Usuário (Roles fixas)

## Conjunto fechado (`Core/Application/Common/Roles.cs`)

Mesmo padrão de `StatusConta`/`FormasPagamento`:

```csharp
public static class Roles
{
    public const string Admin = "Admin";
    public const string Mecanico = "Mecânico";
    public const string Recepcionista = "Recepcionista";
    public const string Financeiro = "Financeiro";

    private static readonly HashSet<string> Validas = new()
    {
        Admin, Mecanico, Recepcionista, Financeiro
    };

    public static bool EhValida(string? valor) => valor is not null && Validas.Contains(valor);
}
```

Nenhuma mudança de entidade/coluna — `Usuario.Role` já é `string`, só
passa a ser validado contra esse conjunto no Handler. Sem migration.

## Create — mudança de contrato de retorno

`CreateUsuarioCommand` hoje retorna `UsuarioDto?` (`null` = e-mail já
em uso). Adicionar uma segunda causa de falha (role inválida) exige
distinguir os dois casos — não dá mais pra usar só `null`. Novo
contrato, no mesmo espírito de `ResultadoAtualizacaoUsuario`:

```csharp
public enum ResultadoCriacaoUsuario
{
    Sucesso,
    EmailEmUso,
    RoleInvalida
}

public class CriarUsuarioResultado
{
    public ResultadoCriacaoUsuario Resultado { get; set; }
    public UsuarioDto? Usuario { get; set; }
}
```

`CreateUsuarioCommand : IRequest<CriarUsuarioResultado>`.
`CreateUsuarioHandler`: valida `Role` **antes** de checar e-mail
duplicado ou não — ordem não importa pro resultado final (são
condições independentes), mas valida `Roles.EhValida(request.Role)`
logo no início por simplicidade, retorna `RoleInvalida` se falhar sem
tocar no repositório.

`ContasPagarController`... (não, aqui é `UsuariosController`):

```csharp
[HttpPost]
public async Task<IActionResult> Create(CreateUsuarioCommand command)
{
    var resultado = await _mediator.Send(command);
    return resultado.Resultado switch
    {
        ResultadoCriacaoUsuario.Sucesso =>
            CreatedAtAction(nameof(GetById), new { id = resultado.Usuario!.Id }, resultado.Usuario),
        ResultadoCriacaoUsuario.EmailEmUso =>
            Conflict(new { message = "Já existe um usuário cadastrado com este e-mail." }),
        ResultadoCriacaoUsuario.RoleInvalida =>
            BadRequest(new { message = "Role inválida. Use Admin, Mecânico, Recepcionista ou Financeiro." }),
        _ => throw new InvalidOperationException($"Resultado inesperado: {resultado.Resultado}")
    };
}
```

## Update — reaproveita o enum existente

`ResultadoAtualizacaoUsuario` ganha mais um caso:

```csharp
public enum ResultadoAtualizacaoUsuario
{
    Sucesso,
    NaoEncontrado,
    EmailEmUso,
    RoleInvalida
}
```

`UpdateUsuarioHandler`: valida `Roles.EhValida(request.Role)` depois de
confirmar que o usuário existe (mesma ordem "existência antes de regra
de negócio" já usada em `UpdateContaReceberHandler` pro cliente).
`UsuariosController.Update` ganha o novo `case` no switch já existente,
mesma mensagem de erro do Create.

## Riscos e Decisões

- **Decisão:** validação por código, sem FluentValidation (constitution,
  seção 6) — mesmo padrão de `FormasPagamento`/`ClienteId`.
- **Risco:** mudar o tipo de retorno de `CreateUsuarioCommand` de
  `UsuarioDto?` pra `CriarUsuarioResultado` é uma mudança de contrato
  interno (Command/Handler), não do contrato HTTP externo — `201`/`409`
  continuam iguais, só ganha um `400` novo. O hub trata qualquer erro
  não-2xx via `onError` do `useMutation` + toast, então não quebra.

## Estratégia de Verificação

- `dotnet build` sem erros.
- Via `curl`:
  - Criar usuário com `role` válida (`"Mecânico"`) → `201`.
  - Criar usuário com `role` inválida (`"Estagiário"`) → `400`.
  - Atualizar usuário existente com `role` inválida → `400`.
  - Atualizar usuário existente com `role` válida → `204`.
  - Critérios já existentes (e-mail duplicado `409`, id inexistente
    `404`) continuam funcionando.
  - Usuário de teste removido depois.
