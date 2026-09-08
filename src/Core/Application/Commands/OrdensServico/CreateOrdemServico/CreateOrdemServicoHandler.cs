using MediatR;
using connectasys_api.Core.Application.DTOs;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.OrdensServico.CreateOrdemServico
{
    public class CreateOrdemServicoHandler : IRequestHandler<CreateOrdemServicoCommand, OrdemServicoDto?>
    {
        private readonly IOrdemServicoRepository _repository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public CreateOrdemServicoHandler(
            IOrdemServicoRepository repository,
            IClienteRepository clienteRepository,
            IVeiculoRepository veiculoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _clienteRepository = clienteRepository;
            _veiculoRepository = veiculoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<OrdemServicoDto?> Handle(CreateOrdemServicoCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
            if (cliente is null) return null;

            var veiculo = await _veiculoRepository.GetByIdAsync(request.VeiculoId);
            if (veiculo is null || veiculo.ClienteId != request.ClienteId) return null;

            if (request.TecnicoId is not null && await _usuarioRepository.GetByIdAsync(request.TecnicoId.Value) is null)
                return null;

            var ordemServico = new OrdemServico
            {
                ClienteId = request.ClienteId,
                VeiculoId = request.VeiculoId,
                TecnicoId = request.TecnicoId,
                DescricaoProblema = request.DescricaoProblema,
                PrevisaoTermino = request.PrevisaoTermino,
                ValorMaoDeObra = request.ValorMaoDeObra,
                Desconto = Math.Clamp(request.Desconto, 0, 100),
                DataAbertura = DateTime.UtcNow
            };

            await _repository.AddAsync(ordemServico);

            return OrdemServicoDto.DaEntidade(ordemServico);
        }
    }
}
