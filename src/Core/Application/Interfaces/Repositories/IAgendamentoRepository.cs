using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Interfaces.Repositories
{
    public interface IAgendamentoRepository
    {
        Task<List<Agendamento>> GetAllAsync();
        Task<Agendamento?> GetByIdAsync(int id);
        Task<List<Agendamento>> GetByTecnicoIdAsync(Guid tecnicoId, DateTime? data = null);
        Task<Agendamento?> GetConflitoAsync(Guid tecnicoId, DateTime slotInicio, int? ignorarId = null);
        Task AddAsync(Agendamento agendamento);
        Task UpdateAsync(Agendamento agendamento);
        Task DeleteAsync(Agendamento agendamento);
    }
}
