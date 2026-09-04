namespace connectasys_api.Core.Application.Common
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Mecanico = "Mecânico";
        public const string Recepcionista = "Recepcionista";
        public const string Financeiro = "Financeiro";

        private static readonly HashSet<string> Validas = new()
        {
            Admin, Mecanico, Recepcionista, Financeiro
        };

        public static bool EhValida(string? valor) => valor is not null && Validas.Contains(valor);
    }
}
