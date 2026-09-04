# Tasks — Documento e Endereço do Cliente

- [x] Adicionar `Cpf`, `Cnpj`, `RazaoSocial`, `Cep`, `Logradouro`,
      `Bairro`, `Municipio`, `Uf` (`string?`) em `Cliente`
      (`Core/Domain/Entities/Cliente.cs`)
- [x] Mapear as 8 colunas novas em `ClienteConfiguration.cs`
      (snake_case, `MaxLength` conforme `design.md`)
- [x] Adicionar os 8 campos em `ClienteDto`
- [x] `CreateClienteCommand` + `CreateClienteHandler`: adicionar os 8
      campos
- [x] `UpdateClienteCommand` + `UpdateClienteHandler`: adicionar os 8
      campos
- [x] `GetAllClientesHandler`/`GetClienteByIdHandler`: incluir os 8
      campos no mapeamento pra `ClienteDto` (não estava no design
      original, mas era necessário pro critério de aceite "GET retorna
      os campos novos")
- [x] Gerar migration (`20260904115813_AddDocumentoEnderecoCliente`)
- [x] Aplicar migration localmente (`dotnet ef database update`)
- [x] `dotnet build` sem erros
- [x] Testar via `curl`:
  - [x] Criar cliente com `cpf` preenchido → `201`, campos salvos
  - [x] Criar cliente com `cnpj`, `razaoSocial` e endereço completo →
        `201`, campos salvos
  - [x] Criar cliente sem nenhum campo novo → `201`, campos `null`
        (compatibilidade com o fluxo atual)
  - [x] Atualizar cliente adicionando `cpf`/endereço depois de criado
        sem eles → `204`, `GET` confirma os novos valores
  - [x] `GET /api/Clientes` e `GET /api/Clientes/{id}` retornam os 8
        campos novos
  - [x] Cliente de teste removido depois

**Status: concluído.** Testado ponta a ponta via API real (3 clientes
de teste criados — CPF, CNPJ, mínimo —, update de endereço/documento
confirmado via GET, todos removidos após os testes).

## Checagem de duplicidade de CPF/CNPJ (adicionado depois)

- [x] `IClienteRepository`/`ClienteRepository`: `GetByCpfAsync`,
      `GetByCnpjAsync`
- [x] `CreateClienteCommand`/`Handler`: retorna `null` quando
      `Cpf`/`Cnpj` já existe em outro cliente
- [x] `ClientesController.Create`: `409 Conflict` quando o resultado é
      `null`
- [x] `UpdateClienteCommand`/`Handler`: novo enum
      `UpdateClienteResult` (`Success`/`ClienteNotFound`/
      `DocumentoEmUso`), ignora o próprio id na checagem
- [x] `ClientesController.Update`: switch com `409 Conflict` pro caso
      `DocumentoEmUso`
- [x] `dotnet build` sem erros
- [x] Testado via `curl`: criar com CNPJ já existente → `409`; update
      pro CNPJ de outro cliente → `409`; update mantendo o próprio CPF
      → `204` (sem falso positivo); registro de teste removido depois

**Status: concluído.**
