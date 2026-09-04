# Design — Documento e Endereço do Cliente

## Entidade (`Core/Domain/Entities/Cliente.cs`)

```csharp
public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string? Cnpj { get; set; }
    public string? RazaoSocial { get; set; }
    public string? Cep { get; set; }
    public string? Logradouro { get; set; }
    public string? Bairro { get; set; }
    public string? Municipio { get; set; }
    public string? Uf { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}
```

Campos novos são `string?` (projeto usa `Nullable` habilitado),
espelhando o padrão opcional já usado em `ContaPagar.FormaPagamento`.

## Tabela / mapeamento (`ClienteConfiguration.cs`)

Colunas novas em `clientes`, snake_case, todas opcionais:

| Propriedade | Coluna | MaxLength |
|---|---|---|
| Cpf | cpf | 11 |
| Cnpj | cnpj | 14 |
| RazaoSocial | razao_social | 200 |
| Cep | cep | 8 |
| Logradouro | logradouro | 200 |
| Bairro | bairro | 100 |
| Municipio | municipio | 100 |
| Uf | uf | 2 |

## DTO (`Core/Application/DTOs/ClienteDto.cs`)

Espelha a entidade — adiciona os mesmos 8 campos novos como `string?`.

## Commands (CQRS)

- `CreateClienteCommand`: adiciona os 8 campos novos (todos
  opcionais); `CreateClienteHandler` passa tudo direto pra entidade,
  sem validação extra (fora de escopo desta spec).
- `UpdateClienteCommand`: mesmos 8 campos novos.
  `UpdateClienteHandler` atualiza tudo, inclusive limpando pra `null`
  quando vier vazio (mesmo comportamento dos campos já existentes).

## Checagem de duplicidade

- `IClienteRepository` ganha `GetByCpfAsync`/`GetByCnpjAsync`.
- `CreateClienteCommand` passa a ser `IRequest<ClienteDto?>` (era
  `IRequest<ClienteDto>`) — `CreateClienteHandler` retorna `null`
  quando o `Cpf` ou `Cnpj` informado já pertence a outro cliente;
  `ClientesController.Create` traduz `null` em `409 Conflict`. Mesmo
  padrão de `CreateVeiculoHandler`/`VeiculosController` (retorno
  nulo → `Controller` decide o código HTTP).
- `UpdateClienteCommand` passa a ser `IRequest<UpdateClienteResult>`
  (era `IRequest<bool>`), com o enum `UpdateClienteResult { Success,
  ClienteNotFound, DocumentoEmUso }` declarado no mesmo arquivo —
  mesmo padrão de `UpdateContaPagarResult`/`UpdateVeiculoResult`. A
  checagem de duplicidade no update ignora o próprio cliente sendo
  editado (`existente.Id != request.Id`).

## Migration

Uma migration nova (`AddDocumentoEnderecoCliente` ou nome similar)
adicionando as 8 colunas opcionais em `clientes`. Sem dado a migrar —
todas as linhas existentes ficam com essas colunas `null`.

## Fora de escopo desta implementação

- Nenhuma chamada a serviço externo (BrasilAPI/ViaCEP) acontece no
  backend — fica inteiramente no `connectasys-hub`, que só envia os
  campos já resolvidos no `POST`/`PUT` de `/api/Clientes`.
