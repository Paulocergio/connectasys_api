# Design — Segurança de Transporte e Postura Pós-Quântica

## 1. Postura Pós-Quântica (registro formal)

| Primitiva | Onde é usada | Tipo | Exposta a Shor? | Exposta a Grover? | Situação |
|---|---|---|---|---|---|
| HMAC-SHA256 | Assinatura do JWT (`TokenService.cs`) | Simétrica | Não (Shor só quebra fatoração/log discreto, base de RSA/ECC) | Sim, mas com ganho só quadrático | Segura hoje: 256 bits de saída dão ~128 bits de margem pós-quântica, padrão aceito |
| BCrypt | Hash de senha (`PasswordHasher.cs`) | Função de hash com custo, não é matemática de chave pública | Não | Sim, ganho quadrático, mitigado pelo custo computacional intencional do algoritmo | Sem mudança necessária |
| RSA / ECC / ECDSA / X.509 próprios | — | — | — | — | **Não usados em nenhum lugar do código da aplicação.** A única superfície assimétrica do sistema é o certificado TLS da conexão HTTPS, que é gerado e gerenciado pela camada de hospedagem/infra, não pelo código deste repositório |

**Conclusão:** não existe hoje, e esta feature não introduz, nenhuma chave assimétrica de longa duração no código da aplicação. Por isso, não há justificativa para adotar bibliotecas de assinatura/troca de chave pós-quântica (ex. Dilithium/ML-DSA, Kyber/ML-KEM) na aplicação — não existe o que substituir. O único ponto onde o algoritmo de Shor seria relevante é a troca de chaves do TLS na conexão HTTPS, e essa negociação é feita pela pilha de TLS do servidor/proxy que termina a conexão (Kestrel, ou um proxy reverso/CDN na frente dele em produção), não por código C# neste repositório.

**Recomendação operacional (fora do código-fonte):** ao decidir a hospedagem de produção, dar preferência a um provedor/proxy que já suporte troca de chaves híbrida pós-quântica em TLS (ex. X25519+ML-KEM — já suportado por navegadores modernos e por CDNs como Cloudflare). Isso elimina o risco de *harvest-now-decrypt-later* sem exigir nenhuma mudança de código aqui. Ver pergunta em aberto na `spec.md` sobre onde a API será hospedada.

## 2. HTTPS — religar

`Program.cs` já tem `app.UseHttpsRedirection()` escrito, só comentado:

```csharp
// antes
// app.UseHttpsRedirection();

// depois
app.UseHttpsRedirection();
```

O perfil de execução `https` já existe em `launchSettings.json` (`https://localhost:7074;http://localhost:5283`) — nenhuma mudança necessária ali. Passa a ser o perfil padrão usado em desenvolvimento (`dotnet run` sem `--launch-profile http`).

Adicionado também `app.UseHsts()` fora do ambiente de desenvolvimento (padrão do template ASP.NET, reforça ao navegador para nunca tentar HTTP de novo):

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
```

Pré-requisito local (ação manual do desenvolvedor, não é código): `dotnet dev-certs https --trust`, uma vez por máquina, para o navegador confiar no certificado de desenvolvimento.

## 3. Segredos fora do repositório

### `API/connectasys_api.API.csproj` — habilitar user-secrets

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <UserSecretsId>a3f8c2d1-7b4e-4a9f-9c2e-1d6b8f0a5e3c</UserSecretsId>
</PropertyGroup>
```

Com `UserSecretsId` presente, `WebApplication.CreateBuilder` carrega automaticamente `dotnet user-secrets` em ambiente `Development` — nenhuma mudança extra em `Program.cs`.

### Comandos (rodados uma vez, localmente — não versionados)

```bash
dotnet user-secrets init --project src/API
dotnet user-secrets set "Jwt:Key" "<256 bits aleatórios, gerados na hora>" --project src/API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=127.0.0.1;Port=5432;Database=ConnectaSysDb;Username=postgres;Password=123456" --project src/API
```

Os secrets ficam em `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json` (fora do repositório, já fora do alcance do Git independente de `.gitignore`).

### `appsettings.json` — remove o valor real, mantém a chave pra documentar o formato esperado

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=127.0.0.1;Port=5432;Database=ConnectaSysDb;Username=postgres;Password="
  },
  "Jwt": {
    "Key": "",
    "Issuer": "ConnectaSysApi",
    "Audience": "ConnectaSysApi",
    "ExpiresMinutes": 60
  },
  ...
}
```

Efeito colateral desejado: se alguém rodar a API sem configurar os user-secrets, `Jwt:Key` chega vazio em `TokenService`, e `SymmetricSecurityKey` lança exceção na primeira tentativa de gerar token — falha visível e imediata, em vez de aceitar silenciosamente uma chave fraca.

Em produção, os mesmos valores são fornecidos por variável de ambiente (`Jwt__Key`, `ConnectionStrings__DefaultConnection` — sintaxe de configuração hierárquica do ASP.NET Core), configurada no provedor de hospedagem escolhido; não é código deste repositório.

## 4. Sem mudança de schema

Nenhuma entidade, migration ou tabela muda nesta feature — é só configuração/infraestrutura de app.
