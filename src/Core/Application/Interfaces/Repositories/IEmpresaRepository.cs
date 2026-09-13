using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Interfaces.Repositories
{
    public interface IEmpresaRepository
    {
        Task<Empresa?> GetByIdAsync(Guid id);
        Task AddAsync(Empresa empresa);

        // Cria a Empresa e o primeiro Usuário (Admin) numa transação só —
        // cadastro self-service não pode deixar uma Empresa órfã sem
        // nenhum usuário se algo falhar no meio do caminho.
        Task CriarComPrimeiroUsuarioAsync(Empresa empresa, Usuario usuario);

        // Apaga TODO dado da empresa (clientes, veículos, OS e itens,
        // estoque, contas, agendamentos, usuários) e a própria empresa,
        // numa transação só. Definitivo — sem soft-delete, sem backup.
        // Chamado quando o trial expira (login ou qualquer acesso
        // autenticado seguinte) — ver LoginHandler e o middleware de
        // trial em Program.cs.
        Task ApagarTudoAsync(Guid empresaId);
    }
}
