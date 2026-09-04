namespace connectasys_api.Core.Application.Common
{
    public static class FormasPagamento
    {
        public const string Cartao = "Cartão";
        public const string Pix = "Pix";
        public const string Boleto = "Boleto";
        public const string Dinheiro = "Dinheiro";

        private static readonly HashSet<string> Validas = new()
        {
            Cartao, Pix, Boleto, Dinheiro
        };

        public static bool EhValida(string? valor) => valor is not null && Validas.Contains(valor);
    }
}
