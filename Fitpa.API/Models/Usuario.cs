using System.Text.Json.Serialization;

namespace Fitpa.API.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Campos para o MFA
        public string? TotpSecret { get; set; }
        public bool IsMfaEnabled { get; set; } = false;

        // Relação 1:N com as pesagens
        [JsonIgnore] // Evita referência circular durante a serialização
        public ICollection<Pesagem> Pesagens { get; set; } = new List<Pesagem>();
    }
}