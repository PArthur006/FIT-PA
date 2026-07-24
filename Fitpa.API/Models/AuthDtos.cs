namespace Fitpa.API.Models
{
    /*
     * Registro
     * Transporta os dados necessários para criar um novo usuário.
     */
    public class RegistroDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /*
     * Login
     * Transporta os dados usados na autenticação e no fluxo de MFA.
     */
    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;       
        public string? MfaCode { get; set; }
        public string? TrustToken { get; set; }
    }

    /*
     * Ativação de MFA
     * Transporta o código informado pelo aplicativo autenticador.
     */
    public class MfaAtivarDto
    {
        public string Codigo { get; set; } = string.Empty;
    }
}