namespace connectasys_api.Core.Application.Common
{
    public static class TiposVeiculo
    {
        public const string Carro = "Carro";
        public const string Moto = "Moto";
        public const string Caminhao = "Caminhão";
        public const string Outros = "Outros";

        private static readonly HashSet<string> Validos = new()
        {
            Carro, Moto, Caminhao, Outros
        };

        public static bool EhValido(string? valor) => valor is not null && Validos.Contains(valor);
    }
}
