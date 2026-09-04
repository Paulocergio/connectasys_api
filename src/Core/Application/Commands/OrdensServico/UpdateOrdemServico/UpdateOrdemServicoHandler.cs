using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Interfaces.Repositories;

namespace connectasys_api.Core.Application.Commands.OrdensServico.UpdateOrdemServico
{
    public class UpdateOrdemServicoHandler : IRequestHandler<UpdateOrdemServicoCommand, UpdateOrdemServicoResult>
    {
        private readonly IOrdemServicoRepository _repository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public UpdateOrdemServicoHandler(
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

        public async Task<UpdateOrdemServicoResult> Handle(UpdateOrdemServicoCommand request, CancellationToken cancellationToken)
        {
            var ordemServico = await _repository.GetByIdAsync(request.Id);
            if (ordemServico is null) return UpdateOrdemServicoResult.OrdemServicoNotFound;

            if (!StatusOrdemServico.EhValido(request.Status)) return UpdateOrdemServicoResult.StatusInvalido;

            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
            if (cliente is null) return UpdateOrdemServicoResult.ClienteOuVeiculoInvalido;

            var veiculo = await _veiculoRepository.GetByIdAsync(request.VeiculoId);
            if (veiculo is null || veiculo.ClienteId != request.ClienteId) return UpdateOrdemServicoResult.ClienteOuVeiculoInvalido;

            if (request.TecnicoId is not null && await _usuarioRepository.GetByIdAsync(request.TecnicoId.Value) is null)
                return UpdateOrdemServicoResult.TecnicoInvalido;

            ordemServico.ClienteId = request.ClienteId;
            ordemServico.VeiculoId = request.VeiculoId;
            ordemServico.TecnicoId = request.TecnicoId;
            ordemServico.Status = request.Status;
            ordemServico.DescricaoProblema = request.DescricaoProblema;
            ordemServico.Diagnostico = request.Diagnostico;
            ordemServico.Solucao = request.Solucao;
            ordemServico.PrevisaoTermino = request.PrevisaoTermino;
            ordemServico.DataConclusao = request.DataConclusao;
            ordemServico.ValorMaoDeObra = request.ValorMaoDeObra;
            ordemServico.Desconto = request.Desconto;
            ordemServico.AprovacaoClienteEm = request.AprovacaoClienteEm;
            ordemServico.AprovacaoClienteNome = request.AprovacaoClienteNome;

            await _repository.UpdateAsync(ordemServico);
            return UpdateOrdemServicoResult.Success;
        }
    }
}
