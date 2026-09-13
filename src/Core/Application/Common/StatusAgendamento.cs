namespace connectasys_api.Core.Application.Common
{
    public static class StatusAgendamento
    {
        public const string Agendado = "Agendado";
        public const string Concluido = "Concluído";
        public const string Cancelado = "Cancelado";

        private static readonly HashSet<string> Validos = new()
        {
            Agendado, Concluido, Cancelado
        };

        public static bool EhValido(string? valor) => valor is not null && Validos.Contains(valor);
    }
}
