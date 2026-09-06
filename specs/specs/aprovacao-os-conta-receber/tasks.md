# Tasks — Conclusão de OS gera Conta a Receber (backend)

## Fase 1 — Banco de dados

- [x] Adicionar `OrdemServicoId` (`int?`) em `ContaReceber.cs`
- [x] `ContaReceberConfiguration.cs` (coluna `ordem_servico_id`, FK →
      `ordens_servico.id`, `DeleteBehavior.Cascade`)
- [x] Migration `20260905220549_AddOrdemServicoIdToContasReceber`
- [x] Migration `20260905223004_ContaReceberCascadeAoDeletarOS`
      (corrige `SetNull` → `Cascade`, ver Fase 4)
- [x] Aplicadas localmente
- [x] `dotnet build` sem erros

**Fase 1: concluída.**

## Fase 2 — CQRS

- [x] `ContaReceberDto`: `OrdemServicoId`, mapeado nos 4 pontos que
      constroem o DTO
- [x] `IContaReceberRepository.ExisteParaOrdemServicoAsync`
- [x] `IOrdemServicoRepository.SalvarComContaReceberAsync`
      (transacional)
- [x] `UpdateOrdemServicoHandler`: detecta transição pra "Concluído",
      gera `ContaReceber` (se valor > 0 e ainda não existir uma pra
      essa OS)
- [x] `dotnet build` sem erros

**Fase 2: concluída.**

## Fase 3 — Verificação (versão com endpoint dedicado de aprovação)

- [x] Aprovar OS com valor > 0 → conta criada corretamente
- [x] Aprovar de novo → `409`, sem duplicar
- [x] Aprovar OS cancelada → `400`
- [x] Aprovar OS valor zero → sem conta
- [x] `PUT` genérico ignorava campos de aprovação
- [x] Limpeza de dados de teste

**Fase 3: concluída, depois revertida (ver Fase 4).**

## Fase 4 — Correção de rumo (feedback do usuário)

Depois da primeira entrega, o usuário pediu duas mudanças:

1. O gatilho não deve ser uma ação separada de "aprovar" — deve ser
   a **mudança de status pra "Concluído"** no fluxo normal de edição.
2. Excluir uma OS deve excluir a conta a receber gerada por ela (não
   deixar órfã).

- [x] Removido `AprovarOrdemServicoCommand`/`Handler` e o endpoint
      `POST /api/OrdensServico/{id}/aprovar`
- [x] `AprovacaoClienteEm`/`AprovacaoClienteNome` voltaram pro
      `UpdateOrdemServicoCommand`/`Handler` (editáveis via `PUT`
      genérico, sem relação com a conta a receber)
- [x] Lógica de geração movida pra dentro de
      `UpdateOrdemServicoHandler`, disparada pela transição de status
- [x] `IOrdemServicoRepository.AprovarAsync` renomeado pra
      `SalvarComContaReceberAsync` (mesma implementação transacional)
- [x] FK de `ContaReceber.OrdemServicoId` trocada de `SetNull` pra
      `Cascade` (nova migration)
- [x] `dotnet build` sem erros

### Verificação (comportamento final)

- [x] Criar OS (mão de obra 200) → `PUT` mudando status pra
      "Concluído" com `dataConclusao` → `204`; conta a receber criada
      (`ordemServicoId` correto, valor 200, vencimento =
      `dataConclusao` + 30 dias)
- [x] Salvar a mesma OS de novo com status "Concluído" → `204`, sem
      duplicar (continua só 1 conta nova)
- [x] Criar OS com valor zero, concluir → `204`, nenhuma conta gerada
- [x] Excluir a OS que gerou conta → a conta some junto (cascade
      confirmado via `GET /api/ContasReceber` antes/depois)
- [x] Conta pré-existente sem vínculo (`ordemServicoId: null`) não foi
      afetada por nenhum teste
- [x] Registros de teste removidos depois (2 OS, 1 usuário local)

**Fase 4: concluída.**

## Fase 5 — Sincronização de valor (feedback do usuário)

Pedido adicional: se o valor da OS mudar depois de gerar a conta, a
conta precisa refletir isso.

- [x] `IContaReceberRepository.GetByOrdemServicoIdAsync` (substitui o
      `ExisteParaOrdemServicoAsync` da Fase 4 — mesma checagem, mas
      retornando a entidade em vez de `bool`)
- [x] `IOrdemServicoRepository.SalvarComContaReceberAsync` ganha um
      segundo parâmetro opcional (`contaReceberExistente`)
- [x] `UpdateOrdemServicoHandler`: sincroniza `Valor` da conta
      existente sempre que a OS é salva (não só na transição pra
      Concluído)
- [x] `AddItemOrdemServicoHandler`: sincroniza `Valor` após adicionar
      item (com recarga da OS pra evitar contar o item novo duas
      vezes — bug encontrado e corrigido durante o teste manual)
- [x] `RemoveItemOrdemServicoHandler`: sincroniza `Valor` após remover
      item
- [x] `dotnet build` sem erros

### Verificação

- [x] Criar OS (mão de obra 100), concluir → conta com valor 100
- [x] Editar OS pra mão de obra 250 → conta atualizada pra 250
- [x] Adicionar item (2×30=60) → conta atualizada pra 310 (bug de
      duplicação encontrado nesse teste e corrigido: EF fixup já
      incluía o item novo na coleção `Itens`, somar de novo duplicava
      pra 370 — corrigido recarregando a OS antes de somar)
- [x] Remover o item → conta volta pra 250
- [x] Excluir a OS → conta some junto (cascade continua funcionando)
- [x] Dados reais do usuário (OS #13, #14 e respectivas contas,
      criados via navegador durante os testes) não foram afetados
- [x] Registros de teste e usuário local removidos depois

**Fase 5: concluída.**

## Fase 6 — Cancelamento remove a conta (feedback do usuário)

Pedido adicional: se uma OS for cancelada, a conta a receber gerada
por ela precisa ser removida (não só ficar "esquecida" pendente).

- [x] `IOrdemServicoRepository.SalvarComContaReceberAsync` ganha um
      terceiro parâmetro opcional (`contaReceberParaRemover`)
- [x] `UpdateOrdemServicoHandler`: se `Status == "Cancelado"`, a conta
      vinculada (se existir) é passada pra remoção — checado antes da
      lógica de sincronizar/criar valor
- [x] `dotnet build` sem erros

### Verificação

- [x] Criar OS (150), concluir → conta criada (150)
- [x] Cancelar a mesma OS → conta desaparece de `GET /api/ContasReceber`
- [x] Registros de teste removidos depois

**Fase 6: concluída.**

**Status geral: concluído**, com o comportamento final descrito no
`spec.md`/`design.md` atuais. Migrations aplicadas localmente — falta
aplicar `AddOrdemServicoIdToContasReceber` e
`ContaReceberCascadeAoDeletarOS` em produção (Azure) no próximo deploy
dessa versão (Fase 6 não precisou de migration nova, só código).
