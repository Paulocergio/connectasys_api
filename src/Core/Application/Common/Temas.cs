namespace connectasys_api.Core.Application.Common
{
    public static class Temas
    {
        public const string Light = "light";
        public const string Dark = "dark";

        private static readonly HashSet<string> Validos = new()
        {
            Light, Dark
        };

        public static bool EhValido(string? valor) => valor is not null && Validos.Contains(valor);
    }
}
