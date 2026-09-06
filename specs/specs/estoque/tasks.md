# Tasks — Estoque (backend)

## Fase 1 — Banco de dados

- [x] Criar entidade `Core/Domain/Entities/Estoque.cs`
- [x] Adicionar `EstoqueId` (`int?`) em `ItemOrdemServico.cs`
- [x] Criar `Infrastructure/Persistence/Configurations/EstoqueConfiguration.cs`
- [x] Atualizar `ItemOrdemServicoConfiguration.cs` (coluna `estoque_id`,
      FK → `estoque.id`, `DeleteBehavior.SetNull`)
- [x] `AppDbContext`: `DbSet<Estoque> Estoque`
- [x] Migration `20260905235448_AddEstoque` gerada e aplicada localmente
- [x] `dotnet build` sem erros

**Fase 1: concluída.**

## Fase 2 — CRUD de Estoque

- [x] `EstoqueDto`
- [x] `IEstoqueRepository` + `EstoqueRepository`
- [x] `CreateEstoqueCommand`/`Handler`
- [x] `UpdateEstoqueCommand`/`Handler`
- [x] `DeleteEstoqueCommand`/`Handler`
- [x] `GetAllEstoqueQuery`/`Handler`
- [x] `GetEstoqueByIdQuery`/`Handler`
- [x] `EstoqueController` (`[Authorize]`)
- [x] Registrado `IEstoqueRepository` no `Program.cs`
- [x] `dotnet build` sem erros

**Fase 2: concluída.** (Namespaces usam `Estoques`, plural, pra não
colidir com o nome do tipo `Estoque` dentro dos próprios arquivos —
mesmo padrão já usado em `ContasPagar`/`OrdensServico`, plural na
pasta, singular na entidade.)

## Fase 3 — Integração com Ordem de Serviço

- [x] `ItemOrdemServicoDto`: `EstoqueId`
- [x] `OrdemServicoDto.DaEntidade`: mapeia `EstoqueId` dos itens
- [x] `AddItemOrdemServicoCommand`: `EstoqueId` (`int?`)
- [x] Novo `AddItemOrdemServicoResultado` (enum) e
      `AddItemOrdemServicoResult` (wrapper)
- [x] `AddItemOrdemServicoHandler`: busca estoque se `EstoqueId`
      informado, valida quantidade, usa descrição/valor do estoque
      como fonte de verdade, debita quantidade
- [x] `RemoveItemOrdemServicoHandler`: devolve quantidade ao estoque
- [x] `DeleteOrdemServicoHandler`: devolve quantidade de todos os
      itens vinculados antes de excluir a OS
- [x] `OrdensServicoController.AddItem`: atualizado pro novo contrato
- [x] `dotnet build` sem erros

**Fase 3: concluída.**

## Fase 4 — Verificação

- [x] Criar peça no estoque → `201`
- [x] Adicionar item vinculado (qtd 3, disponível 10) → `200`,
      descrição/valor vieram do estoque (ignorando o que o cliente
      mandou), estoque debitado pra 7
- [x] Pedir quantidade maior que o disponível (100 de 7) → `400`,
      estoque inalterado
- [x] Remover o item → estoque restaurado (7 → 10)
- [x] Adicionar de novo (qtd 4) e excluir a OS inteira → estoque
      restaurado (6 → 10)
- [x] Adicionar item sem `estoqueId` (fluxo antigo) → funciona normal,
      `estoqueId: null` no retorno
- [x] Remover peça do estoque → funciona (`204`), sem travar por
      referências antigas
- [x] Registros de teste removidos depois (2 OS, 1 peça de estoque, 1
      usuário local)

**Fase 4: concluída.**

**Status geral: concluído.** Testado ponta a ponta via API local real
(Postgres local), cobrindo CRUD completo de Estoque e os 3 pontos de
integração (adicionar item, remover item, excluir OS). Migration
aplicada localmente — falta aplicar em produção (Azure) no próximo
deploy.

## Fase 5 — Revisão de campos (feedback do usuário)

Pedido adicional: preço de compra separado do preço de venda (com
margem de lucro calculada), Nome/Descrição separados, estoque mínimo
por peça com aviso de estoque baixo.

- [x] `Estoque`: `Descricao` → `Nome` (obrigatório) + `Descricao`
      (opcional, nullable); `ValorUnitario` → `PrecoVenda`; novo
      `PrecoCompra`; novo `EstoqueMinimo`
- [x] Como a migration `AddEstoque` da Fase 1 ainda não tinha ido pra
      produção, foi **refeita do zero** (revertida + removida +
      regerada) em vez de empilhar uma segunda migration
- [x] `EstoqueDto`, `Create`/`UpdateEstoqueCommand`/`Handler`,
      `GetAllEstoque`/`GetEstoqueByIdHandler` atualizados pros campos
      novos
- [x] `AddItemOrdemServicoHandler`: usa `estoque.Nome` (descrição do
      item) e `estoque.PrecoVenda` (valor unitário) em vez dos campos
      antigos
- [x] Margem de lucro **não é persistida** — sempre calculada
      (`(PrecoVenda - PrecoCompra) / PrecoCompra * 100`), decisão
      documentada no `design.md`
- [x] `dotnet build` sem erros

### Verificação

- [x] Criar peça (compra 50, venda 70, mínimo 5, qtd 3) → `201` com
      todos os campos novos
- [x] Adicionar item de OS vinculado → descrição = nome da peça, valor
      = preço de venda (não o de compra)
- [x] Registros de teste removidos depois

**Fase 5: concluída.**
