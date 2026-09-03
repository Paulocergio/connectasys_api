# Spec — Segurança de Transporte e Postura Pós-Quântica

## Objetivo

Fechar as lacunas atuais de segurança de transporte da API (HTTPS desligado, segredo do JWT fraco e versionado, credenciais de banco em texto puro) e documentar formalmente a postura do projeto em relação a ataques de computação quântica — registrando o que já está protegido, o que precisa de ajuste, e o que fica fora do escopo do código da aplicação.

## Contexto — por que agora

O usuário pediu uma revisão de segurança quântica. O levantamento (ver `design.md` desta feature para o detalhe técnico) mostrou que a aplicação não usa nenhuma criptografia assimétrica própria — só HMAC-SHA256 (simétrico) para assinar o JWT e BCrypt para hash de senha, ambos já resistentes ao algoritmo de Grover nos tamanhos de chave em uso hoje. Não há, portanto, nada assimétrico no código pra "trocar por um algoritmo pós-quântico".

O risco real e acionável é outro: **o HTTPS está desligado** (`app.UseHttpsRedirection()` comentado em `Program.cs`), o que significa que hoje não existe nenhuma cifra de transporte, quântica ou clássica — o tráfego entre o hub e a API trafega em texto puro. Religar HTTPS é pré-requisito para qualquer proteção contra "capturar tráfego hoje, decifrar no futuro" (*harvest-now-decrypt-later*) fazer sentido.

## Critérios de aceite

- A API só aceita conexões HTTPS (redirecionamento de HTTP para HTTPS ativo), funcionando localmente com o certificado de desenvolvimento do .NET.
- A chave de assinatura do JWT (`Jwt:Key`) deixa de ser uma string fixa versionada no repositório; passa a vir de configuração local não versionada (user-secrets/variável de ambiente), com entropia adequada (256 bits aleatórios).
- A senha de conexão do PostgreSQL deixa de estar em texto puro em `appsettings.json` versionado; passa a vir de configuração local não versionada.
- Existe, no `design.md` desta feature, um registro formal: quais primitivas criptográficas o projeto usa (HMAC-SHA256, BCrypt), por que continuam seguras diante dos algoritmos de Shor e Grover, e que a troca de chaves híbrida pós-quântica em TLS é responsabilidade da camada de hospedagem/proxy reverso de produção — fora do código-fonte deste repositório.
- `dotnet build` sem erros após as mudanças.
- Fluxo de desenvolvimento local (rodar a API + testar via Swagger, e via o hub local) continua funcionando depois de religar o HTTPS.

## Fora de escopo (por enquanto)

- Configurar troca de chaves híbrida pós-quântica (ex. X25519+ML-KEM) no proxy reverso ou provedor de hospedagem de produção — decisão operacional de infraestrutura ainda não definida; fica registrada como recomendação no `design.md`, não implementada aqui.
- Gestão de segredos via cofre gerenciado (Azure Key Vault, AWS Secrets Manager etc.) — infraestrutura de produção ainda não definida; por ora os segredos saem do arquivo versionado e vão para configuração local não versionada (user-secrets / variável de ambiente).
- Proteger os endpoints existentes (`Clientes`, `Usuarios`, `Veiculos`, `ContasPagar`, `ContasReceber`) com `[Authorize]` — já registrado como fora de escopo na spec de [autenticacao](../autenticacao/spec.md). É um gap real, mas não é sobre transporte/quântico; fica para uma spec própria.
- Qualquer biblioteca de assinatura ou troca de chave pós-quântica na aplicação (ex. Dilithium/ML-DSA para assinar o JWT) — não há hoje nenhuma chave assimétrica de longa duração no código a proteger, e o ecossistema .NET (`JwtBearer`) não tem suporte de produção para algoritmos de assinatura PQC ainda. Reavaliar se isso mudar.
- Refresh token, rotação automática de segredos, rate limiting, MFA.

## Suposições e perguntas em aberto

- Suposição: o certificado de desenvolvimento HTTPS padrão do .NET (`dotnet dev-certs https --trust`) é suficiente para o ambiente local, sem custo de configuração adicional para o usuário.
- Pergunta em aberto: onde a API será hospedada em produção? Sem essa definição, a recomendação de PQC-hybrid em TLS fica genérica no `design.md`, sem apontar um provedor específico.
