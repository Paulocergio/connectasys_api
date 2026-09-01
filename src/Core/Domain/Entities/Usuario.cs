using System;
using System.Collections.Generic;
using System.Text;

namespace connectasys_api.Core.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime DataCriacaoUtc { get; set; } = DateTime.UtcNow;
        public string Telefone { get; set; } = string.Empty;
    }
}
