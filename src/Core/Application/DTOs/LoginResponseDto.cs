namespace connectasys_api.Core.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraEmUtc { get; set; }
        public Guid UsuarioId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Tema { get; set; } = "light";
        public DateTime TrialExpiraEmUtc { get; set; }
    }
}
