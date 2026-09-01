# Constitution — ConnectaSys API

> Este documento define as regras não-negociáveis do projeto. Toda spec nova (`/specs/<feature>/spec.md`) deve respeitar o que está aqui. Se uma feature exigir quebrar uma regra, a mudança deve ser discutida e refletida **neste arquivo primeiro**, não decidida silenciosamente durante a implementação.

## 1. Propósito do sistema

ConnectaSys é um SaaS de gestão para oficinas mecânicas. O núcleo do domínio gira em torno de **clientes**, **usuários** (equipe da oficina) e, futuramente, **veículos** e **ordens de serviço**.

## 2. Arquitetura

- O projeto segue **Clean Architecture**, com 3 projetos na solução: `Core`, `Infrastructure`, `API`.
- `Core` não tem dependência de nenhum outro projeto. `Infrastructure` depende de `Core`. `API` depende dos dois.
- Dentro de `Core`, a divisão é `Domain` (entidades) e `Application` (DTOs, Commands, Queries, Handlers, interfaces de repositório).
- **CQRS via MediatR** é o padrão obrigatório para toda ação de escrita ou leitura que passe pela API:
  - Escrita → `Command` + `Handler` em `Core/Application/Commands/<Entidade>/<Ação>/`
  - Leitura → `Query` + `Handler` em `Core/Application/Queries/<Entidade>/<Ação>/`
  - Controllers **nunca** acessam o `DbContext` diretamente — só conhecem `IMediator`.
- Acesso a dados passa sempre por um **Repository**: interface em `Core/Application/Interfaces/Repositories`, implementação em `Infrastructure/Persistence/Repositories`.

## 3. Banco de dados

- **PostgreSQL**, acessado via Npgsql + Entity Framework Core.
- Mapeamento explícito com Fluent API (`IEntityTypeConfiguration<T>`) em `Infrastructure/Persistence/Configurations` — nunca depender de convenção implícita do EF para nomes de coluna.
- Colunas do banco em **snake_case** (`data_criacao_utc`, `senha_hash`, `empresa_id`); propriedades C# em **PascalCase**. O mapeamento entre os dois é sempre feito via `.HasColumnName(...)`.
- Migrations ficam em `Infrastructure/Persistence/Migrations` e **são versionadas no Git** (diferente de `bin/`, `obj/` etc.).
- Não usar `DROP TABLE` manual em tabela que já tem migration aplicada — preferir `dotnet ef migrations remove` ou `dotnet ef database drop` para manter o histórico do EF sincronizado com o banco real.

## 4. Nomenclatura

- Entidades de domínio, DTOs e rotas de API em **português** (`Cliente`, `Usuario`, `/api/Clientes`), porque o domínio de negócio é em português.
- Termos de arquitetura ficam em **inglês** (`Command`, `Query`, `Handler`, `Repository`, `DTO`), seguindo a convenção da própria stack (.NET/MediatR).
- Namespace raiz: `connectasys_api.{Core|Infrastructure|API}.*`.

## 5. API

- ASP.NET Core Web API com **Controllers** (não minimal API).
- Documentação via **Swagger/Swashbuckle**, exposta em `/swagger` (ambiente de desenvolvimento).
- Retornos da API usam **DTOs**, nunca a entidade de domínio direto — evita vazar campos internos (ex: `SenhaHash`) e desacopla o contrato da API do modelo de dados.

## 6. Estado atual — o que ainda NÃO existe (de propósito)

Estas ausências são decisões conscientes da fase atual do projeto, não esquecimento:

- ❌ Sem autenticação/autorização (JWT ainda não implementado)
- ❌ Sem hash de senha (senha é gravada em texto puro por enquanto)
- ❌ Sem validação de entrada (FluentValidation ainda não adicionado)
- ❌ Sem testes automatizados
- ❌ Sem multi-tenant funcional (coluna `empresa_id` existe, mas não é usada pra filtrar nada ainda)

Qualquer spec nova pode assumir esse estado **a menos que a própria spec seja sobre implementar um desses itens**.

## 7. Estrutura de pastas

Só são criadas as pastas realmente usadas no momento. Pastas do "esqueleto" original (`ValueObjects`, `Enums`, `Domain/Interfaces`, `UseCases`, `Validators`, `ExternalServices`, `CrossCutting`, `Middlewares`, `Filters`, `Extensions`, `ViewModels`) são criadas **sob demanda**, quando uma spec realmente precisar delas — não antecipadamente.

## 8. Ambiente local

- `UseHttpsRedirection()` está desativado em `Program.cs` para simplificar testes via HTTP no Swagger em desenvolvimento local.
- Connection string fica em `appsettings.json`; segredos locais (senha real, `appsettings.Development.json`) ficam fora do Git via `.gitignore`.

## 9. Como as specs devem ser escritas

Cada feature nova ganha uma pasta em `/specs/<nome-da-feature>/` com 3 arquivos:

1. **`spec.md`** — o quê e por quê. Requisitos em linguagem natural, critérios de aceite, campos envolvidos. Sem detalhe técnico de implementação.
2. **`design.md`** — como. Entidade, DTO, endpoints, decisões técnicas específicas da feature.
3. **`tasks.md`** — checklist ordenado de implementação, quebrado em unidades pequenas o suficiente pra cada uma virar um arquivo ou uma mudança clara.

Nenhuma dessas etapas pula para código antes de existir pelo menos o `spec.md` aprovado.
