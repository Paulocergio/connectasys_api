using MediatR;

namespace connectasys_api.Core.Application.Commands.ContasReceber.DeleteContaReceber
{
    public class DeleteContaReceberCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
