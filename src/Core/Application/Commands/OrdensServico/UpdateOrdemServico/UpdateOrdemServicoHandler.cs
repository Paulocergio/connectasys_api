using MediatR;
using connectasys_api.Core.Application.Common;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Domain.Entities;

namespace connectasys_api.Core.Application.Commands.OrdensServico.UpdateOrdemServico
{
    public class UpdateOrdemServicoHandler : IRequestHandler<UpdateOrdemServicoCommand, UpdateOrdemServicoResult>
    {
        private readonly IOrdemServicoRepository _repository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IContaReceberRepository _contaReceberRepository;

        public UpdateOrdemServicoHandler(
            IOrdemServicoRepository repository,
            IClienteRepository clienteRepository,
            IVeiculoRepository veiculoRepository,
            IUsuarioRepository usuarioRepository,
            IContaReceberRepository contaReceberRepository)
        {
            _repository = repository;
            _clienteRepository = clienteRepository;
            _veiculoRepository = veiculoRepository;
            _usuarioRepository = usuarioRepository;
            _contaReceberRepository = contaReceberRepository;
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

            var statusAnterior = ordemServico.Status;

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

            var acabouDeConcluir = statusAnterior != StatusOrdemServico.Concluido
                && ordemServico.Status == StatusOrdemServico.Concluido;

            var contaExistente = await _contaReceberRepository.GetByOrdemServicoIdAsync(ordemServico.Id);

            var valorTotal = ordemServico.ValorMaoDeObra
                + ordemServico.Itens.Sum(i => i.Quantidade * i.ValorUnitario)
                - ordemServico.Desconto;

            ContaReceber? contaNova = null;
            ContaReceber? contaParaAtualizar = null;
            ContaReceber? contaParaRemover = null;

            if (ordemServico.Status == StatusOrdemServico.Cancelado)
            {
                // OS cancelada não pode ter cobrança pendente — remove a
                // conta a receber gerada por ela, se existir.
                contaParaRemover = contaExistente;
            }
            else if (contaExistente is not null)
            {
                // Conta já gerada por esta OS — mantém o valor sincronizado
                // com qualquer edição de mão de obra/desconto.
                contaExistente.Valor = valorTotal;
                contaParaAtualizar = contaExistente;
            }
            else if (acabouDeConcluir && valorTotal > 0)
            {
                var dataBase = ordemServico.DataConclusao ?? DateTime.UtcNow;
                var descricao = $"OS #{ordemServico.Id} — {ordemServico.DescricaoProblema}";
                contaNova = new ContaReceber
                {
                    ClienteId = ordemServico.ClienteId,
                    Descricao = descricao.Length > 200 ? descricao[..200] : descricao,
                    Valor = valorTotal,
                    DataVencimento = dataBase.AddDays(30),
                    DataCadastro = DateTime.UtcNow,
                    OrdemServicoId = ordemServico.Id
                };
            }

            await _repository.SalvarComContaReceberAsync(ordemServico, contaNova, contaParaAtualizar, contaParaRemover);
            return UpdateOrdemServicoResult.Success;
        }
    }
}
