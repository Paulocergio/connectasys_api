# Spec — Documento e Endereço do Cliente

## Objetivo

Permitir que o cadastro de cliente registre o documento (CPF ou CNPJ)
e os dados de endereço, para que a oficina saiba se está lidando com
pessoa física ou jurídica e tenha o endereço completo salvo. O
preenchimento automático desses campos a partir de CNPJ (BrasilAPI) ou
CEP (ViaCEP) é feito pelo `connectasys-hub` no momento do cadastro —
esta spec cobre apenas os campos que a API passa a aceitar e
armazenar.

## User stories

- Como atendente, quero informar o CPF (pessoa física) ou o CNPJ
  (pessoa jurídica) do cliente no cadastro.
- Como atendente, quero que o endereço do cliente (logradouro, bairro,
  município, UF, CEP) fique salvo no cadastro dele.
- Como atendente, quero que a razão social fique salva quando o
  cliente for pessoa jurídica.

## Campos novos

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| Cpf | texto | não | Só dígitos, até 11 caracteres. Preenchido quando o cliente é pessoa física. |
| Cnpj | texto | não | Só dígitos, até 14 caracteres. Preenchido quando o cliente é pessoa jurídica. |
| RazaoSocial | texto | não | Só relevante quando `Cnpj` está preenchido. |
| Cep | texto | não | Só dígitos, até 8 caracteres. |
| Logradouro | texto | não | |
| Bairro | texto | não | |
| Municipio | texto | não | |
| Uf | texto | não | 2 caracteres. |

Todos os campos novos são opcionais na API — o cliente mínimo
(Nome/Email/Telefone) continua válido sem eles. Decidir se o cadastro
deve *exigir* CPF ou CNPJ é responsabilidade da tela do hub, não desta
spec (ver "Fora de escopo").

## Critérios de aceite

- Criar ou atualizar cliente com um `Cpf`/`Cnpj` que já pertence a
  outro cliente retorna `409 Conflict` com mensagem explicando que já
  existe cliente cadastrado com aquele documento — o cadastro não é
  duplicado.
- Criar cliente informando `Cpf` (e nenhum `Cnpj`) retorna `201` com
  os dados salvos.
- Criar cliente informando `Cnpj`, `RazaoSocial` e os campos de
  endereço retorna `201` com todos os dados salvos.
- Criar cliente sem nenhum campo novo continua funcionando como hoje
  (todos ficam `null`).
- Atualizar cliente aceita os mesmos campos novos.
- Listar/buscar cliente por id retorna os campos novos no DTO (`null`
  quando não preenchidos).

## Fora de escopo (por enquanto)

- Chamar BrasilAPI ou ViaCEP — isso é feito pelo `connectasys-hub` no
  frontend; a API do ConnectaSys só recebe e guarda o resultado.
- Validar formato/dígito verificador de CPF ou CNPJ (ver constitution,
  seção 6 — sem validação de entrada por enquanto).
- Exigir que exatamente um entre `Cpf`/`Cnpj` esteja preenchido — por
  enquanto são só dois campos opcionais independentes (a checagem de
  duplicidade roda em qualquer um dos dois que vier preenchido).
- Detectar automaticamente se o cliente é pessoa física ou jurídica a
  partir de outro campo — a distinção é só "qual dos dois campos veio
  preenchido".
- Suporte a múltiplos endereços por cliente, ou a campos de endereço
  adicionais (número, complemento) — não solicitados agora.
