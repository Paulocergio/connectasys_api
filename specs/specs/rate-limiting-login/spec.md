# Spec — Rate Limiting no Login

## Objetivo

O teste de força bruta feito nesta sessão (3.000 tentativas em ~1,9s,
sem nenhum bloqueio) mostrou que `POST /api/Auth/login` aceita volume
ilimitado de tentativas. Esta spec fecha essa brecha combinando duas
proteções: limite por IP (contém varredura em massa) e bloqueio por
conta após tentativas falhas repetidas (contém ataque direcionado a um
e-mail específico, mesmo vindo de IPs diferentes).

## User stories

- Como dono da oficina, quero que um atacante não consiga testar
  milhares de senhas por segundo contra o login.
- Como usuário legítimo, quero continuar conseguindo logar
  normalmente mesmo com esse limite (não devo ser bloqueado por errar
  a senha uma ou duas vezes).

## Critérios de aceite

- Um mesmo IP que faça mais de 10 requisições a `/api/Auth/login` em
  60 segundos recebe `429 Too Many Requests` a partir da 11ª.
- Uma mesma conta (e-mail) que acumule 5 tentativas de login com senha
  errada em uma janela de 15 minutos passa a receber `429` em
  qualquer tentativa seguinte contra esse e-mail, por 15 minutos —
  mesmo vindo de um IP diferente do que gerou as tentativas.
- Um login com credenciais corretas *zera* o contador de tentativas
  falhas daquela conta.
- Login legítimo (poucas tentativas, dentro do limite) continua
  funcionando exatamente como hoje — sem fricção perceptível pro
  usuário normal.
- Os dois limites (IP e conta) são independentes: estourar um não
  afeta o outro, e qualquer um dos dois sendo estourado já barra a
  requisição.

## Fora de escopo (por enquanto)

- Rate limiting nos outros endpoints da API (Clientes, Veículos,
  Contas) — só o login, que é o vetor de força bruta.
- Persistência dos contadores entre reinicializações da API (usa
  cache em memória do processo — reiniciar a API zera os contadores;
  aceitável pro estágio atual, single-instance, dev/local).
- CAPTCHA ou qualquer verificação de humano.
- Notificar o usuário real por e-mail quando a conta dele é
  bloqueada.
- Corrigir o canal lateral de tempo (achado separado, já documentado,
  fica pra outra spec).
