using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Interfaces.Repositories
{
    public interface IContaPagarRepository
    {
        Task<List<ContaPagar>> GetAllAsync();
        Task<ContaPagar?> GetByIdAsync(int id);
        Task AddAsync(ContaPagar contaPagar);
        Task UpdateAsync(ContaPagar contaPagar);
        Task DeleteAsync(ContaPagar contaPagar);
    }
}
