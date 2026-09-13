# Spec — Multitenant (Empresa + cadastro self-service + trial)

**Data:** 2026-09-13 · **Status:** implementado

> A constitution (seção 6) citava "coluna `empresa_id` existe, mas não
> é usada pra filtrar nada" — isso estava desatualizado: não existia
> nenhuma coluna `empresa_id`, nenhuma entidade `Empresa`, e nenhum
> isolamento de dados entre oficinas. Todo mundo (todas as oficinas
> cadastradas até aqui) compartilhava o mesmo pool de clientes,
> veículos, OS, estoque etc. Esta spec cobre a implementação real,
> do zero.

## Objetivo

Cada oficina que usa o ConnectaSys passa a ser uma **Empresa** (tenant)
isolada — usuários, clientes, veículos, ordens de serviço, estoque,
contas e agendamentos de uma oficina nunca aparecem pra outra. Uma
oficina nova se cadastra sozinha (self-service, sem precisar de
ninguém do time do ConnectaSys), aceita um contrato explícito, e ganha
um período de teste que **expira no início do 2º dia depois do
cadastro** (cadastrou dia 13 → bloqueado a partir do dia 15). Vencido
o prazo, **todo o dado da empresa é apagado automaticamente e sem
volta** — não existe cobrança automatizada nem processo de recuperação.

> **Revisão 2026-09-13 (correção do usuário):** a primeira versão desta
> spec calculava o vencimento como `DataCadastro + 3 dias exatos` (72h
> a partir do horário do cadastro) e só bloqueava o login, sem apagar
> nada. O usuário corrigiu com um exemplo concreto ("cadastrar hoje dia
> 13, dia 15 tem que expirar") e pediu a exclusão automática dos dados
> nesse momento, com um contrato de aceite avisando disso no cadastro.
> Esta revisão substitui as duas coisas.

## User stories

- Como dono de oficina, quero me cadastrar sozinho no site e já
  começar a usar o sistema, sem esperar ninguém liberar meu acesso.
- Como dono de oficina, quero alguns dias pra testar antes de decidir
  se vou usar de verdade.
- Como dono de oficina, antes de me cadastrar, quero saber claramente
  que meus dados serão apagados se eu não contratar a assinatura
  depois do teste — sem surpresa.
- Como dono de oficina, não quero que meus dados (clientes, OS,
  financeiro) apareçam pra nenhuma outra oficina que usa o sistema, e
  vice-versa.
- Como oficina que já usava o sistema antes dessa mudança, quero que
  meus dados continuem exatamente como estavam, sem precisar recadastrar
  nada.

## Campos

### Empresa (nova)

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Id | Guid | sim (automático) | |
| Nome | texto | sim | nome da oficina |
| DataCadastro | data/hora | não (automático) | |
| TrialExpiraEm | data/hora | sim | dia calendário do cadastro + 2 dias, à meia-noite UTC — ver Critérios de aceite |

### Toda entidade de negócio existente ganha `EmpresaId`

`Usuario`, `Cliente`, `Veiculo`, `OrdemServico`, `ItemOrdemServico`,
`Estoque`, `ContaPagar`, `ContaReceber`, `Agendamento` — todas ganham
uma referência obrigatória a `Empresa`.

## Critérios de aceite

- Cadastro self-service (`POST /api/Auth/registrar`) cria a Empresa e
  o primeiro usuário (perfil Admin) numa operação só, e já devolve um
  token de login (auto-login, sem passo extra).
- `TrialExpiraEm` é calculado por **dia calendário**, contando o
  próprio dia do cadastro como dia 1: cadastro em qualquer horário do
  dia 13 → `TrialExpiraEm = 15/09 00:00 UTC` → a partir do instante em
  que vira dia 15, o acesso já está bloqueado. Não é "72 horas exatas
  a partir do horário do cadastro".
- Cadastro com e-mail já em uso retorna erro claro (`409`), sem criar
  nada.
- Login de um usuário cuja empresa já passou do `TrialExpiraEm` é
  recusado (`402`) com mensagem clara — a pessoa não entra, mesmo com
  senha certa — **e todo o dado da empresa é apagado nesse momento**
  (ver critério de exclusão abaixo).
- Qualquer requisição autenticada (não só login) feita com um token
  emitido antes do vencimento, mas usado depois dele, também é
  recusada (`402`) e dispara a mesma exclusão — cobre a janela em que
  um token ainda tecnicamente válido (validade de até 60 min) poderia
  ser usado depois do trial vencer.
- **Exclusão de dados:** ao detectar o vencimento (login ou qualquer
  acesso autenticado seguinte), o sistema apaga **imediatamente e sem
  confirmação adicional**: todos os clientes, veículos, ordens de
  serviço (e itens), peças de estoque, contas a pagar/receber,
  agendamentos e usuários da empresa, e por fim a própria empresa.
  Definitivo — sem soft-delete, sem lixeira, sem backup. Uma segunda
  tentativa de acesso com as mesmas credenciais falha como "e-mail ou
  senha inválidos" (a conta genuinamente não existe mais).
- Todo dado (cliente, veículo, OS, peça de estoque, conta, agendamento)
  criado por um usuário só aparece pra usuários da mesma empresa —
  listar, buscar por id, editar ou remover um registro de outra
  empresa se comporta como se ele não existisse (`404`, nunca `403` —
  não revela que o registro existe em outro tenant).
- CPF/CNPJ de cliente é único **por empresa**, não globalmente — duas
  oficinas diferentes podem cadastrar o mesmo cliente real (mesmo
  documento) sem conflito.
- E-mail de usuário continua único **em todo o sistema** (não por
  empresa) — é por ele que o login descobre a qual empresa a pessoa
  pertence, então não pode haver duas contas com o mesmo e-mail em
  empresas diferentes.
- Um técnico (`TecnicoId` em Ordens de Serviço/Agendamentos) só pode
  ser um usuário da mesma empresa de quem está criando o registro —
  referenciar um usuário de outra empresa é tratado como técnico
  inválido, do mesmo jeito que um id inexistente.
- Todo dado que já existia antes desta feature (clientes, usuários,
  OS etc. cadastrados antes do multitenant) continua acessível
  normalmente — foi todo migrado pra uma empresa "legada" com trial
  bem no futuro (não expira na prática).
- O cadastro self-service (lado do hub, ver `specs/multitenant/` do
  `connectasys-hub`) só permite enviar o formulário depois de marcar
  uma caixa de aceite de um contrato que explica claramente a
  exclusão de dados — a API em si não impõe isso (não há campo
  "aceitouTermos" no `RegistrarCommand`), é uma trava do front.

## Fora de escopo (por enquanto)

- Qualquer cobrança/assinatura automatizada — liberar uma empresa
  depois do trial expirado é uma ação manual (`UPDATE` direto no
  banco), não um fluxo do sistema. Não existe integração de pagamento.
- Múltiplas empresas por usuário (um e-mail pertence a uma única
  empresa).
- Convite de novos usuários pra uma empresa por e-mail — criar um
  usuário adicional continua sendo feito por um Admin já logado na
  tela de Usuários (`POST /api/Usuarios`, sem mudança de fluxo, só
  passou a herdar a empresa de quem está criando).
- Qualquer aviso/contador de dias restantes de trial na interface — o
  backend devolve `trialExpiraEmUtc` no login, mas não existe nenhuma
  UI usando isso além de guardar no estado da sessão.
- Customização de subdomínio por empresa (`oficina.connectasys.com.br`)
  — todo mundo continua acessando o mesmo domínio, a separação é 100%
  por dado (via token), não por URL.

## Suposições e Perguntas em Aberto

- Decidido com o usuário: depois do trial expirado, o login é
  **totalmente bloqueado** (não uma versão só-leitura) — mensagem
  clara, sem acesso a nada.
- Decidido com o usuário (revisão 2026-09-13): a exclusão dos dados é
  **automática e imediata** no momento em que o vencimento é
  detectado, não uma ação manual posterior — ver Critérios de aceite.
  A alternativa considerada (só bloquear o acesso, mantendo o dado
  guardado até alguém apagar manualmente) foi descartada.
- Suposição: o primeiro usuário criado no cadastro self-service é
  sempre `Role = Admin` — faz sentido, é quem está criando a conta da
  oficina.
- Suposição: telefone é obrigatório no cadastro self-service (mesmo
  padrão já usado no cadastro de usuário/cliente existente).
