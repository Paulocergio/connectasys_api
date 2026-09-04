# 🔧 ConnectaSys API

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=flat&logo=postgresql&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=flat&logo=swagger&logoColor=black)
![Status](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)

API do **ConnectaSys**, um SaaS de gestão para oficinas mecânicas. O objetivo do sistema é centralizar o controle de clientes, usuários (mecânicos, atendentes, administradores) e, futuramente, ordens de serviço, veículos e histórico de manutenção de cada oficina.

> ⚠️ Projeto em desenvolvimento inicial. Autenticação, validações e regras de negócio mais avançadas ainda serão adicionadas.

## 🛠️ Tecnologias

- ⚙️ **.NET 10** / C#
- 🐘 **PostgreSQL**
- 🗃️ **Entity Framework Core** (Npgsql)
- 📨 **MediatR** — implementação do padrão CQRS
- 📘 **Swagger / Swashbuckle** — documentação e testes da API

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture**, dividido em 3 projetos:

```
connectasys_api.sln
├── src/Core            → Domínio e regras de aplicação (não depende de nada externo)
│   ├── Domain           → Entidades (Cliente, Usuario)
│   └── Application      → DTOs, Commands, Queries, Handlers (CQRS) e interfaces de repositório
│
├── src/Infrastructure   → Implementações concretas
│   └── Persistence       → DbContext, mapeamentos (EF Core), migrations e repositórios
│
└── src/API              → Camada de apresentação
    └── Controllers        → Endpoints HTTP, recebem a requisição e delegam ao MediatR
```

**🔄 Fluxo de uma requisição:** `Controller → IMediator → Handler → Repository → PostgreSQL`

O Controller não acessa o banco diretamente — ele envia um Command (escrita) ou Query (leitura) para o MediatR, que localiza o Handler correspondente e executa a lógica de negócio.

## ✅ Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) rodando localmente ou acessível pela rede

## 🚀 Como rodar o projeto

1. **📥 Clone o repositório e restaure os pacotes:**
   ```bash
   git clone <url-do-repositorio>
   cd connectasys_api
   dotnet restore
   ```

2. **⚙️ Configure a connection string** em `src/API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=127.0.0.1;Port=5432;Database=ConnectaSysDb;Username=postgres;Password=SUA_SENHA"
   }
   ```

3. **🗃️ Aplique as migrations** (cria as tabelas no banco):
   ```bash
   dotnet ef database update --project src/Infrastructure/connectasys_api.Infrastructure.csproj --startup-project src/API/connectasys_api.API.csproj
   ```

4. **▶️ Rode a API:**
   ```bash
   cd src/API
   dotnet run
   ```

5. **📘 Acesse o Swagger** para testar os endpoints:
   ```
   http://localhost:<porta>/swagger
   ```

## 📡 Endpoints disponíveis

### 👤 Clientes — `/api/Clientes`

| Método | Rota | Descrição |
|--------|------|-----------|
| 🟢 GET | `/api/Clientes` | Lista todos os clientes |
| 🟢 GET | `/api/Clientes/{id}` | Busca um cliente pelo id |
| 🟡 POST | `/api/Clientes` | Cria um novo cliente |
| 🔵 PUT | `/api/Clientes/{id}` | Atualiza um cliente existente |
| 🔴 DELETE | `/api/Clientes/{id}` | Remove um cliente |

**Campos:** `nome`, `email`, `telefone`, `dataCadastro`

### 🧑‍🔧 Usuários — `/api/Usuarios`

| Método | Rota | Descrição |
|--------|------|-----------|
| 🟢 GET | `/api/Usuarios` | Lista todos os usuários |
| 🟢 GET | `/api/Usuarios/{id}` | Busca um usuário pelo id |
| 🟡 POST | `/api/Usuarios` | Cria um novo usuário |
| 🔵 PUT | `/api/Usuarios/{id}` | Atualiza um usuário existente |
| 🔴 DELETE | `/api/Usuarios/{id}` | Remove um usuário |

**Campos:** `nome`, `email`, `role`, `telefone`, `dataCriacaoUtc`

**E-mail único:** criar ou atualizar um usuário com um e-mail já usado por
outro retorna `409 Conflict`.

## 🗺️ Roadmap

- [ ] 🔐 Autenticação e autorização (JWT)
- [ ] 🔑 Hash de senha (atualmente sem criptografia — apenas CRUD básico)
- [ ] ✅ Validações de entrada (FluentValidation)
- [ ] 🏢 Multi-tenant por oficina (`empresa_id`)
- [ ] 🚗 Cadastro de veículos e ordens de serviço
- [ ] 🧪 Testes automatizados

## 📄 Licença

Projeto privado — todos os direitos reservados.
