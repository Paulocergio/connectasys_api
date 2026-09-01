namespace connectasys_api.Core.Application.Common
{
    public static class StatusConta
    {
        public const string Paga = "Paga";
        public const string Pendente = "Pendente";
        public const string Atrasada = "Atrasada";

        public static string Calcular(DateTime? dataQuitacao, DateTime dataVencimento) =>
            dataQuitacao is not null
                ? Paga
                : DateTime.UtcNow.Date > dataVencimento.Date
                    ? Atrasada
                    : Pendente;
    }
}
