namespace connectasys_api.Core.Application.Common
{
    public static class StatusOrdemServico
    {
        public const string Aberto = "Aberto";
        public const string EmAndamento = "Em Andamento";
        public const string AguardandoPeca = "Aguardando Peça";
        public const string Concluido = "Concluído";
        public const string Cancelado = "Cancelado";

        private static readonly HashSet<string> Validos = new()
        {
            Aberto, EmAndamento, AguardandoPeca, Concluido, Cancelado
        };

        public static bool EhValido(string? valor) => valor is not null && Validos.Contains(valor);
    }
}
