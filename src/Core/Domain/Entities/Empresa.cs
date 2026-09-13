namespace connectasys_api.Core.Domain.Entities
{
    public class Empresa
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        // Fim do período de teste — passado esse instante, login é bloqueado
        // (ver LoginHandler). Sem integração de pagamento ainda: liberar uma
        // empresa depois do trial é uma ação manual (UPDATE direto), não um
        // fluxo do sistema.
        public DateTime TrialExpiraEm { get; set; }
    }
}
