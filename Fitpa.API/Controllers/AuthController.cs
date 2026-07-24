using Fitpa.API.Data;
using Fitpa.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OtpNet;
using Microsoft.AspNetCore.Authorization;

namespace Fitpa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /*
         * Registro
         * Verifica se o usuário já existe e cria a conta com a senha criptografada.
         */
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(RegistroDto request)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Usuário já existe.");
            
            var usuario = new Usuario
            {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new {mensagem = "Usuário registrado com sucesso."});
        }

        /*
         * Login
         * Valida credenciais, verifica MFA quando necessário e retorna os tokens.
         */
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });

            bool isDispositivoConfiavel = false;
            if (!string.IsNullOrEmpty(request.TrustToken))
            {
                isDispositivoConfiavel = IsTrustTokenValido(request.TrustToken, usuario.Username);
            }

            if (usuario.IsMfaEnabled && !isDispositivoConfiavel)
            {
                if (string.IsNullOrEmpty(request.MfaCode))
                    return Unauthorized(new { requiresMfa = true, mensagem = "Código MFA obrigatório." });

                var totp = new Totp(Base32Encoding.ToBytes(usuario.TotpSecret));
                bool isValido = totp.VerifyTotp(request.MfaCode, out long timeStepMatched, window: new VerificationWindow(2, 2));

                if (!isValido)
                    return Unauthorized(new { mensagem = "Código MFA inválido." });
            }

            var token = CriarTokenJwt(usuario);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var trustTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Username),
                    new Claim("TokenType", "MfaTrust")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var trustToken = tokenHandler.WriteToken(tokenHandler.CreateToken(trustTokenDescriptor));

            return Ok(new { token, trustToken });
        }

        /*
         * Token JWT
         * Monta o token principal com as claims do usuário autenticado.
         */
        private string CriarTokenJwt(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15), // Token expira em 15 minutos
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        /*
         * Gerar MFA
         * Cria a chave secreta, salva no banco e devolve a URI do QR Code.
         */
        [Authorize]
        [HttpPost("mfa/gerar")]
        public async Task<IActionResult> GerarChaveMfa()
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimId == null) return Unauthorized();
            var usuarioId = int.Parse(claimId);

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null) return NotFound("Usuário não encontrado.");

            if (usuario.IsMfaEnabled)
                return BadRequest("MFA já está ativado para esta conta.");
            
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(secretKey);

            usuario.TotpSecret = base32Secret;
            await _context.SaveChangesAsync();

            var issuer = "FitPA";
            var uri = $"otpauth://totp/{issuer}:{usuario.Username}?secret={base32Secret}&issuer={issuer}";

            return Ok(new
            {
                mensagem = "Chave gerada. Escaneie o QR Code no app Authenticator.",
                qrCodeUri = uri,
                chaveManual = base32Secret
            });
        }

        /*
         * Ativar MFA
         * Valida o código informado e marca o MFA como ativo para o usuário.
         */
        [Authorize]
        [HttpPost("mfa/ativar")]
        public async Task<IActionResult> AtivarMfa([FromBody] MfaAtivarDto request)
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimId == null) return Unauthorized();
            var usuarioId = int.Parse(claimId);

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null || string.IsNullOrEmpty(usuario.TotpSecret))
                return BadRequest("MFA não foi iniciado.");

            var totp = new Totp(Base32Encoding.ToBytes(usuario.TotpSecret));

            bool isValido = totp.VerifyTotp(request.Codigo, out long timeStepMatched, window: new VerificationWindow(2, 2));

            if (!isValido) return BadRequest("Código MFA inválido.");

            usuario.IsMfaEnabled = true;
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "MFA ativado com sucesso." });
        }

        /*
         * Trust Token
         * Valida se o token informado pertence ao usuário e ainda está dentro do prazo.
         */
        private bool IsTrustTokenValido(string token, string username)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                var tokenUsername = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tokenType = principal.FindFirst("TokenType")?.Value;

                return tokenUsername == username && tokenType == "MfaTrust";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[FALHA TRUST TOKEN]: {ex.Message}\n");
                return false;
            }
        }
    }
}