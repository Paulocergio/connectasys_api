using MediatR;
using connectasys_api.Core.Application.DTOs;

namespace connectasys_api.Core.Application.Commands.Veiculos.CreateVeiculo
{
    public class CreateVeiculoCommand : IRequest<VeiculoDto?>
    {
        public int ClienteId { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Cor { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }
}
