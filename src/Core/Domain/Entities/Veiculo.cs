using connectasys_api.Core.Application.Common;

namespace connectasys_api.Core.Domain.Entities
{
    public class Veiculo
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Cor { get; set; } = string.Empty;
        public string Tipo { get; set; } = TiposVeiculo.Carro;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}
