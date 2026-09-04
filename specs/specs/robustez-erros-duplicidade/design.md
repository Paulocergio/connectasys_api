# Design — Tratamento de Erros e Unicidade de CPF/CNPJ

## 1. Middleware global de tratamento de exceção

`Program.cs`, logo após `var app = builder.Build();` (antes de
qualquer outro middleware, inclusive Swagger) — ativo em **todos** os
ambientes, não só fora de Development, porque o vazamento foi
observado justamente rodando local:

```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        if (feature?.Error is not null)
        {
            logger.LogError(feature.Error, "Erro nao tratado em {Path}", context.Request.Path);
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = "Ocorreu um erro interno. Tente novamente mais tarde." });
    });
});
```

O erro completo (`feature.Error`) vai pro log padrão do ASP.NET Core
(console, visível no terminal onde `dotnet run` está rodando) — o
desenvolvedor não perde informação de debug, só o cliente HTTP deixa
de ver.

## 2. Índice único em `cpf`/`cnpj` (fecha a corrida)

### Migration

`ClienteConfiguration.cs` ganha:

```csharp
builder.HasIndex(c => c.Cpf).IsUnique();
builder.HasIndex(c => c.Cnpj).IsUnique();
```

Postgres já trata múltiplos `NULL` como não-conflitantes num índice
único por padrão — não precisa de filtro extra pra permitir vários
clientes sem documento.

### Repository traduz a violação de constraint

Como `Core` não pode depender de `Npgsql`/`EntityFrameworkCore`
(Artigo 2 da constitution), a tradução do erro de banco pra um erro de
domínio acontece na própria `Infrastructure`:

- Novo `Core/Application/Exceptions/DocumentoDuplicadoException.cs` —
  exceção simples, sem dependência de EF/Npgsql.
- `ClienteRepository.AddAsync`/`UpdateAsync` (`Infrastructure`)
  envolvem o `SaveChangesAsync` num `try/catch`: se a
  `DbUpdateException` tiver uma `PostgresException` com
  `SqlState == PostgresErrorCodes.UniqueViolation`, relança como
  `DocumentoDuplicadoException`; qualquer outra exceção continua
  propagando normal (cai no middleware global acima).

### Handler trata a exceção de domínio

`CreateClienteHandler`/`UpdateClienteHandler` envolvem a chamada ao
repositório: se cair em `DocumentoDuplicadoException`, retornam o
mesmo resultado que já usam hoje pro caso sequencial (`null` no
create, `UpdateClienteResult.DocumentoEmUso` no update) — o
`ClientesController` não muda, o `409` já existe.

## Arquivos alterados

- `src/API/Program.cs` — middleware global de exceção
- `src/Infrastructure/Persistence/Configurations/ClienteConfiguration.cs` — 2 índices únicos
- `src/Core/Application/Exceptions/DocumentoDuplicadoException.cs` — novo
- `src/Infrastructure/Persistence/Repositories/ClienteRepository.cs` — try/catch em `AddAsync`/`UpdateAsync`
- `src/Core/Application/Commands/Clientes/CreateCliente/CreateClienteHandler.cs` — try/catch ao redor do `AddAsync`
- `src/Core/Application/Commands/Clientes/UpdateCliente/UpdateClienteHandler.cs` — try/catch ao redor do `UpdateAsync`
- Migration nova (`AddIndiceUnicoCpfCnpjCliente`)

## Riscos e decisões

- **Decisão:** o middleware de exceção fica ativo em todos os
  ambientes (não só fora de Development) — decisão deliberada dessa
  spec, diferente do que seria "padrão" (deixar a Developer Exception
  Page do ASP.NET Core em dev). Pra uma API JSON pura, a página de
  erro detalhada em HTML não ajuda muito de qualquer forma; o log no
  console já dá o mesmo detalhe pro desenvolvedor.
- **Decisão:** a checagem de duplicidade feita antes do insert (spec
  anterior) continua existindo — ela é o caminho rápido/comum (evita
  ida desnecessária ao banco na maioria dos casos); o índice único é a
  garantia de verdade contra concorrência. As duas camadas juntas.
- **Risco aceito:** validação de tamanho de campo continua ausente —
  o `500` genérico (sem detalhe) é aceitável até o FluentValidation
  chegar; o objetivo aqui é parar de vazar informação, não eliminar
  todo erro 500 possível.

## Estratégia de verificação

- Enviar `uf` com string longa → `500` com mensagem genérica, sem
  stack trace no corpo. Conferir no log do servidor que o erro
  completo apareceu lá.
- Duas requisições de criar cliente com o mesmo CNPJ disparadas em
  paralelo (`Promise.all` ou similar) → uma `201`, outra `409` (nunca
  `500`, nunca as duas `201`).
- Fluxo sequencial de duplicidade (já testado na spec anterior)
  continua `409`.
- `dotnet build` sem erros.
