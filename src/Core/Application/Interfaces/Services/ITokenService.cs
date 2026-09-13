namespace connectasys_api.Core.Application.Interfaces.Services
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiraEmUtc) GerarToken(Guid usuarioId, Guid empresaId, string email, string nome, string role);
    }
}
