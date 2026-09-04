namespace connectasys_api.Core.Application.Interfaces.Services
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiraEmUtc) GerarToken(Guid usuarioId, string email, string nome, string role);
    }
}
