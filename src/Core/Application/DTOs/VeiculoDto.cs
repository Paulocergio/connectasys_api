namespace connectasys_api.Core.Application.DTOs
{
    public class VeiculoDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Cor { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
    }
}
