# Spec — Tratamento de Erros e Unicidade de CPF/CNPJ

## Objetivo

A varredura de segurança autenticada desta sessão encontrou dois
problemas concretos: (1) qualquer exceção não tratada na API devolve
o stack trace completo pro cliente (caminho de arquivo do servidor,
versão de bibliotecas, detalhes do banco), e (2) duas requisições
simultâneas conseguem criar clientes com o mesmo CPF/CNPJ, porque a
checagem de duplicidade (feita numa spec anterior) não é atômica. Esta
spec fecha os dois.

## User stories

- Como dono da oficina, quero que um erro inesperado na API nunca
  exponha detalhes internos do servidor pra quem está chamando.
- Como atendente, quero que seja impossível cadastrar dois clientes
  com o mesmo CPF/CNPJ, mesmo se dois cadastros acontecerem ao mesmo
  tempo.

## Critérios de aceite

- Qualquer exceção não tratada em qualquer endpoint retorna
  `500` com uma mensagem genérica em português (ex.: "Ocorreu um erro
  interno. Tente novamente mais tarde."), sem stack trace, sem caminho
  de arquivo, sem nome de biblioteca — em qualquer ambiente, inclusive
  desenvolvimento local (não só produção).
- O erro completo continua disponível pro desenvolvedor via log do
  servidor (console/terminal onde a API roda).
- Duas requisições de criação de cliente com o mesmo CPF (ou o mesmo
  CNPJ), disparadas ao mesmo tempo, resultam em **uma** criada e a
  outra recusada — nunca as duas.
- A resposta da requisição recusada por duplicidade continua sendo
  `409 Conflict` com a mesma mensagem que já existe hoje pro caso
  sequencial (não pode virar `500` genérico).
- Enviar um campo maior que o tamanho da coluna (ex.: `uf` com mais de
  2 caracteres) continua retornando um erro `500` genérico (validação
  de tamanho de campo em si é fora de escopo aqui — ver seção
  seguinte), mas sem vazar detalhe interno.

## Fora de escopo (por enquanto)

- Validação de tamanho de campo antes de tentar salvar (ex.: rejeitar
  `uf` de 40 caracteres com `400` em vez de deixar chegar no banco) —
  isso é o trabalho de adicionar FluentValidation, já documentado como
  ausência conhecida na constitution; esta spec só garante que,
  quando isso acontecer, o erro não vaza detalhe interno.
- Corrigir a mesma classe de exceção não tratada em outros pontos além
  do handler global (esta spec cobre com um middleware único que
  protege toda a API de uma vez).
- Unicidade de outros campos além de CPF/CNPJ.
