using Microsoft.EntityFrameworkCore;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;
using connectasys_api.Core.Domain.Entities;
using connectasys_api.Infrastructure.Persistence.Context;

namespace connectasys_api.Infrastructure.Persistence.Repositories
{
    public class AgendamentoRepository : IAgendamentoRepository
    {
        private readonly AppDbContext _context;
        private readonly Guid _empresaId;

        public AgendamentoRepository(AppDbContext context, ITenantContext tenantContext)
        {
            _context = context;
            _empresaId = tenantContext.EmpresaId;
        }

        public async Task<List<Agendamento>> GetAllAsync() =>
            await _context.Agendamentos.Where(a => a.EmpresaId == _empresaId).ToListAsync();

        public async Task<Agendamento?> GetByIdAsync(int id) =>
            await _context.Agendamentos.FirstOrDefaultAsync(a => a.Id == id && a.EmpresaId == _empresaId);

        public async Task<List<Agendamento>> GetByTecnicoIdAsync(Guid tecnicoId, DateTime? data = null)
        {
            var query = _context.Agendamentos.Where(a => a.TecnicoId == tecnicoId && a.EmpresaId == _empresaId);
            if (data is not null)
            {
                var dia = data.Value.Date;
                var proximoDia = dia.AddDays(1);
                query = query.Where(a => a.DataHoraInicio >= dia && a.DataHoraInicio < proximoDia);
            }
            return await query.OrderBy(a => a.DataHoraInicio).ToListAsync();
        }

        public async Task<Agendamento?> GetConflitoAsync(Guid tecnicoId, DateTime slotInicio, int? ignorarId = null)
        {
            var query = _context.Agendamentos.Where(a =>
                a.TecnicoId == tecnicoId &&
                a.DataHoraInicio == slotInicio &&
                a.Status == StatusAgendamento.Agendado &&
                a.EmpresaId == _empresaId);

            if (ignorarId is not null)
                query = query.Where(a => a.Id != ignorarId.Value);

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Agendamento agendamento)
        {
            agendamento.EmpresaId = _empresaId;
            _context.Agendamentos.Add(agendamento);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Agendamento agendamento)
        {
            _context.Agendamentos.Update(agendamento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Agendamento agendamento)
        {
            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();
        }
    }
}
