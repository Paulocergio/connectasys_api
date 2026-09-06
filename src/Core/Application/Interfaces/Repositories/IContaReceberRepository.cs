using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Interfaces.Repositories
{
    public interface IContaReceberRepository
    {
        Task<List<ContaReceber>> GetAllAsync();
        Task<ContaReceber?> GetByIdAsync(int id);
        Task<List<ContaReceber>> GetByClienteIdAsync(int clienteId);
        Task<ContaReceber?> GetByOrdemServicoIdAsync(int ordemServicoId);
        Task AddAsync(ContaReceber contaReceber);
        Task UpdateAsync(ContaReceber contaReceber);
        Task DeleteAsync(ContaReceber contaReceber);
    }
}
