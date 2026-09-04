# Tasks — Tratamento de Erros e Unicidade de CPF/CNPJ

- [x] `Program.cs`: middleware global `UseExceptionHandler` (resposta
      genérica em JSON, log do erro completo), ativo em todos os
      ambientes, registrado logo após `builder.Build()`
- [x] `Core/Application/Exceptions/DocumentoDuplicadoException.cs`: nova
- [x] `ClienteConfiguration.cs`: `HasIndex(c => c.Cpf).IsUnique()` e
      `HasIndex(c => c.Cnpj).IsUnique()`
- [x] `ClienteRepository.cs`: try/catch em `AddAsync`/`UpdateAsync`
      traduzindo violação de unique constraint (Postgres) pra
      `DocumentoDuplicadoException`
- [x] `CreateClienteHandler.cs`: try/catch ao redor do `AddAsync`,
      captura `DocumentoDuplicadoException` → retorna `null`
- [x] `UpdateClienteHandler.cs`: try/catch ao redor do `UpdateAsync`,
      captura `DocumentoDuplicadoException` → retorna
      `UpdateClienteResult.DocumentoEmUso`
- [x] Gerar migration (`20260904151415_AddIndiceUnicoCpfCnpjCliente`)
- [x] Aplicar migration localmente
- [x] `dotnet build` sem erros
- [x] Testar:
  - [x] Campo `uf` com string longa → `500` genérico
        (`"Ocorreu um erro interno..."`), sem stack trace no corpo;
        log do servidor mostra o erro completo (confirmado)
  - [x] 5 criações de cliente com o mesmo CNPJ disparadas em paralelo
        → exatamente 1 `201`, as outras 4 `409` (nunca `500`, nunca
        mais de uma criada)
  - [x] Duplicidade sequencial (spec anterior) continua `409`
        (coberto pelo mesmo teste acima)
  - [x] Registro de teste removido depois

**Status: concluído.** Testado ponta a ponta via API real, inclusive
o cenário de concorrência real (5 requisições simultâneas).
