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

## E-mail único (nova etapa)

- [x] Remover as linhas duplicadas de teste (`carlos.santos@empresa.com`)
      do banco local via `DELETE /api/Usuarios/{id}`. Nota: as 10 linhas
      eram indistinguíveis (mesmo nome/e-mail/telefone, lixo de clique
      repetido em teste manual) — as 10 foram removidas em vez de manter
      uma, já que nenhuma carregava dado diferente das outras. Restou só
      o usuário real (`juniorcergio@gmail.com`).
- [x] Adicionar `builder.HasIndex(u => u.Email).IsUnique();` em
      `UsuarioConfiguration.cs`.
- [x] Gerar migration `AddUniqueIndexUsuarioEmail` e aplicar localmente
      (`dotnet ef database update`) — `CREATE UNIQUE INDEX "IX_usuarios_email"
      ON usuarios (email);` confirmado no log da aplicação.
- [x] Mudar `CreateUsuarioCommand` de `IRequest<UsuarioDto>` para
      `IRequest<UsuarioDto?>`; `CreateUsuarioHandler` verifica
      `GetByEmailAsync` antes de criar e retorna `null` se já existir.
- [x] Atualizar `UsuariosController.Create` pra responder `409 Conflict`
      quando o Handler retornar `null`.
- [x] Criar enum `ResultadoAtualizacaoUsuario` (`Sucesso`, `NaoEncontrado`,
      `EmailEmUso`); `UpdateUsuarioCommand` passa a usá-lo como retorno em
      vez de `bool`.
- [x] Atualizar `UpdateUsuarioHandler`: além do `GetByIdAsync` já existente,
      verificar se o novo e-mail já pertence a outro usuário
      (`GetByEmailAsync` + comparar `Id`) e retornar `EmailEmUso` nesse
      caso.
- [x] Atualizar `UsuariosController.Update` pra mapear o enum em
      `204`/`404`/`409`.
- [x] `dotnet build` sem erros (0 avisos, 0 erros).
- [x] Testar manualmente (HTTP direto):
  - [x] Criar usuário com e-mail já existente
        (`juniorcergio@gmail.com`) → `409` com
        `{"message":"Já existe um usuário cadastrado com este e-mail."}`,
        nenhuma linha nova no banco.
  - [x] Atualizar usuário B para o e-mail do usuário A → `409`, nada
        alterado.
  - [x] Atualizar usuário mantendo o próprio e-mail → `204`, funciona
        normalmente.
  - [ ] Tentar inserir duplicata direto no banco (fora da API) — **não
        testado**: sem cliente `psql` disponível no ambiente local usado
        pra essa correção. O `CREATE UNIQUE INDEX` executado com sucesso
        na migration já é a garantia de que o Postgres rejeita duplicata
        em qualquer camada; só a confirmação manual via SQL direto ficou
        pendente.

**Status: concluído** (exceto o teste manual de bypass direto no banco,
marcado acima como não executado por falta de ferramenta no ambiente).
