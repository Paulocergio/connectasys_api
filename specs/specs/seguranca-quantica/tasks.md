# Tasks — Segurança de Transporte e Postura Pós-Quântica

- [x] Adicionar `<UserSecretsId>` ao `API/connectasys_api.API.csproj`
      (via `dotnet user-secrets init`, que já insere a tag automaticamente)
- [x] `dotnet user-secrets init --project src/API`
- [x] Gerar uma chave aleatória de 256 bits e rodar
      `dotnet user-secrets set "Jwt:Key" "<valor gerado>" --project src/API`
- [x] `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=127.0.0.1;Port=5432;Database=ConnectaSysDb;Username=postgres;Password=123456" --project src/API`
- [x] Remover o valor real de `Jwt:Key` e a senha da connection string do
      `appsettings.json` versionado (deixar `Jwt:Key: ""` e
      `Password=` vazio, mantendo as demais chaves como documentação do
      formato esperado)
- [x] Descomentar `app.UseHttpsRedirection()` em `Program.cs` e adicionar
      `app.UseHsts()` fora do ambiente de desenvolvimento
- [x] `dotnet build` sem erros
- [x] Testar localmente (`dotnet run --launch-profile https`; certificado
      já estava confiável, `dotnet dev-certs https --check --trust`
      confirmou):
  - [x] `https://localhost:7074/swagger` carrega normalmente (`200`)
  - [x] Rota HTTP real redireciona pra HTTPS (`307` →
        `https://localhost:7074/...`)
  - [x] Login (`POST /api/Auth/login`) funciona sobre HTTPS e retorna
        token normalmente — testado com usuário temporário criado e
        removido depois (`teste.pqc.temp@example.com`)
  - [x] Rodar a API sem os user-secrets configurados (renomeado
        `secrets.json` temporariamente) e confirmar que falha de forma
        clara (`500`, `SymmetricSecurityKey`... `key length is zero`),
        em vez de aceitar uma chave vazia silenciosamente — secrets
        restaurados e API voltou ao normal (`401` em senha errada)
- [ ] Confirmar que o hub (`connectasys-hub`, feature espelhada) continua
      conseguindo logar contra a API agora em HTTPS

**Status: implementação e testes de backend concluídos. Falta a
verificação de integração com o hub (depende da feature espelhada em
`connectasys-hub`).**
