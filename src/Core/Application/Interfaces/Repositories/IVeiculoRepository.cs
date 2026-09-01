using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Interfaces.Repositories
{
    public interface IVeiculoRepository
    {
        Task<List<Veiculo>> GetAllAsync();
        Task<Veiculo?> GetByIdAsync(int id);
        Task<List<Veiculo>> GetByClienteIdAsync(int clienteId);
        Task AddAsync(Veiculo veiculo);
        Task UpdateAsync(Veiculo veiculo);
        Task DeleteAsync(Veiculo veiculo);
    }
}
