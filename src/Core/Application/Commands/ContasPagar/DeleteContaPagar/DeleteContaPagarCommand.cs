using MediatR;

namespace connectasys_api.Core.Application.Commands.ContasPagar.DeleteContaPagar
{
    public class DeleteContaPagarCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
