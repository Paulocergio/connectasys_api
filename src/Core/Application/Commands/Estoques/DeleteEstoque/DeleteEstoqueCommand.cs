using MediatR;

namespace connectasys_api.Core.Application.Commands.Estoques.DeleteEstoque
{
    public class DeleteEstoqueCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
