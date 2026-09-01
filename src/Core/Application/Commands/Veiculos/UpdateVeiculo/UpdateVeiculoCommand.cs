using MediatR;

namespace connectasys_api.Core.Application.Commands.Veiculos.UpdateVeiculo
{
    public enum UpdateVeiculoResult
    {
        Success,
        VeiculoNotFound,
        ClienteInvalido
    }

    public class UpdateVeiculoCommand : IRequest<UpdateVeiculoResult>
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Cor { get; set; } = string.Empty;
    }
}
