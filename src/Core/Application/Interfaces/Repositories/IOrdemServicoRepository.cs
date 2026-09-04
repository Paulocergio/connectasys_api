using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Interfaces.Repositories
{
    public interface IOrdemServicoRepository
    {
        Task<List<OrdemServico>> GetAllAsync();
        Task<OrdemServico?> GetByIdAsync(int id);
        Task<List<OrdemServico>> GetByClienteIdAsync(int clienteId);
        Task<List<OrdemServico>> GetByVeiculoIdAsync(int veiculoId);
        Task AddAsync(OrdemServico ordemServico);
        Task UpdateAsync(OrdemServico ordemServico);
        Task DeleteAsync(OrdemServico ordemServico);

        Task<ItemOrdemServico?> GetItemByIdAsync(int itemId);
        Task AddItemAsync(ItemOrdemServico item);
        Task RemoveItemAsync(ItemOrdemServico item);
    }
}
