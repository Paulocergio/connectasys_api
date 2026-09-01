using connectasys_api.Core.Application.Interfaces.Services;

namespace connectasys_api.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string senha) => BCrypt.Net.BCrypt.HashPassword(senha);

        public bool Verify(string senha, string hash) => BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}
