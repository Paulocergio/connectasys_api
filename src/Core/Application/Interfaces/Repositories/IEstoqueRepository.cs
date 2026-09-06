using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Interfaces.Repositories
{
    public interface IEstoqueRepository
    {
        Task<List<Estoque>> GetAllAsync();
        Task<Estoque?> GetByIdAsync(int id);
        Task AddAsync(Estoque estoque);
        Task UpdateAsync(Estoque estoque);
        Task DeleteAsync(Estoque estoque);
    }
}
