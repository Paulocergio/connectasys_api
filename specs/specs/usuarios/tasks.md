# Tasks — Usuários

- [x] Recriar entidade `Usuario` em `Domain/Entities` (versão simplificada, só com Nome/Email/Role/Telefone/DataCriacaoUtc)
- [x] Recriar `UsuarioConfiguration` (Fluent API, mapeamento snake_case)
- [x] Criar `UsuarioDto`
- [x] Criar `IUsuarioRepository`
- [x] Criar `UsuarioRepository`
- [x] Criar `CreateUsuarioCommand` + `CreateUsuarioHandler`
- [x] Criar `UpdateUsuarioCommand` + `UpdateUsuarioHandler`
- [x] Criar `DeleteUsuarioCommand` + `DeleteUsuarioHandler`
- [x] Criar `GetAllUsuariosQuery` + `GetAllUsuariosHandler`
- [x] Criar `GetUsuarioByIdQuery` + `GetUsuarioByIdHandler`
- [x] Reescrever `UsuariosController` usando `IMediator` (substituindo acesso direto ao `AppDbContext`)
- [x] Registrar `IUsuarioRepository` no `Program.cs`
- [x] Apagar tabela `usuarios` antiga e as migrations antigas
- [x] Gerar e aplicar migration nova do zero (`InitialCreate`)
- [x] Testar CRUD completo via Swagger

**Status: concluído.** Tabela recriada do zero nessa versão simplificada — ver `spec.md` para o histórico dos campos removidos.

## Hash de senha (nova etapa)

- [x] Adicionar pacote `BCrypt.Net-Next` ao `Infrastructure.csproj`
- [x] Criar `IPasswordHasher` em `Core/Application/Interfaces/Services/IPasswordHasher.cs`
- [x] Criar `PasswordHasher` em `Infrastructure/Security/PasswordHasher.cs` (implementação com BCrypt)
- [x] Registrar `IPasswordHasher` no `Program.cs` (`AddSingleton`)
- [x] Adicionar propriedade `SenhaHash` à entidade `Usuario`
- [x] Atualizar `UsuarioConfiguration` mapeando `SenhaHash` → coluna `senha_hash` (`HasMaxLength(60)`)
- [x] Gerar migration `AddSenhaHashToUsuarios` (coluna não existia na tabela atual — ver `design.md`)
- [x] Aplicar migration localmente (`dotnet ef database update`) — usuários de teste pré-existentes ficaram com `senha_hash` vazio (default da migration); nenhum é dado real, então não recriados
- [x] Adicionar campo `Senha` (obrigatório) ao `CreateUsuarioCommand`
- [x] Atualizar `CreateUsuarioHandler` para injetar `IPasswordHasher` e preencher `SenhaHash` a partir de `request.Senha`
- [x] Adicionar campo `Senha` (`string?`, opcional) ao `UpdateUsuarioCommand`
- [x] Atualizar `UpdateUsuarioHandler` para injetar `IPasswordHasher` e recalcular `SenhaHash` só quando `Senha` for enviada
- [x] Revisar `UsuarioDto` e todos os pontos de retorno da API para confirmar que `Senha`/`SenhaHash` nunca são expostos (checagem de critério de aceite)
- [x] Testar via API (Swagger/HTTP direto):
  - [x] Criar usuário com senha → resposta `201` não contém senha nem hash
  - [x] Conferir no banco que `senha_hash` tem formato BCrypt (`$2a$...`), não texto puro
  - [x] Atualizar usuário sem enviar `Senha` → hash permanece o mesmo
  - [x] Atualizar usuário enviando nova `Senha` → hash muda (novo salt/hash BCrypt, diferente do anterior)
  - [x] Listar e buscar por id → nenhuma resposta contém senha/hash

**Status: concluído.** Login/autenticação (JWT) fica fora desta etapa — spec futura separada, ver `spec.md`.
