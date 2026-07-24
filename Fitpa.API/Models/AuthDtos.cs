namespace Fitpa.API.Models
{
    public class RegistroDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;       
        public string? MfaCode { get; set; }
        public string? TrustToken { get; set; }
    }

    public class MfaAtivarDto
    {
        public string Codigo { get; set; } = string.Empty;
    }
}