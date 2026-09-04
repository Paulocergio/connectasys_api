# Tasks — Perfis de Usuário (Roles fixas)

- [x] Criar `Roles` (`Core/Application/Common/Roles.cs`)
- [x] Criar `ResultadoCriacaoUsuario` + `CriarUsuarioResultado`
- [x] `CreateUsuarioCommand`: mudar retorno de `UsuarioDto?` para `CriarUsuarioResultado`
- [x] `CreateUsuarioHandler`: validar `Role` (retorna `RoleInvalida` se inválida, antes de checar e-mail)
- [x] `UsuariosController.Create`: switch sobre `ResultadoCriacaoUsuario`
- [x] `ResultadoAtualizacaoUsuario`: adicionar caso `RoleInvalida`
- [x] `UpdateUsuarioHandler`: validar `Role` (depois de confirmar que o usuário existe)
- [x] `UsuariosController.Update`: novo `case` no switch existente
- [x] `dotnet build` sem erros
- [x] Testar via `curl`:
  - [x] Criar com `role` válida → `201`
  - [x] Criar com `role` inválida → `400`
  - [x] Atualizar com `role` inválida → `400`
  - [x] Atualizar com `role` válida → `204`
  - [x] E-mail duplicado no create continua `409`; id inexistente no update continua `404`
  - [x] Usuário de teste removido depois

**Status: concluído.** Testado ponta a ponta via API real (usuário de
teste criado, atualizado e removido; regressões de e-mail duplicado e
id inexistente confirmadas intactas).
