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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == request.Username);

            // Transformando a mensagem em um objeto JSON
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
                return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });

            // 1. Verifica se o usuário mandou um TrustToken válido
            bool isDispositivoConfiavel = false;
            if (!string.IsNullOrEmpty(request.TrustToken))
            {
                isDispositivoConfiavel = IsTrustTokenValido(request.TrustToken, usuario.Username);
            }

            // 2. Exige o MFA se o dispositivo não for confiável
            if (usuario.IsMfaEnabled && !isDispositivoConfiavel)
            {
                if (string.IsNullOrEmpty(request.MfaCode))
                    return Unauthorized(new { requiresMfa = true, mensagem = "Código MFA obrigatório." });

                var totp = new Totp(Base32Encoding.ToBytes(usuario.TotpSecret));
                bool isValido = totp.VerifyTotp(request.MfaCode, out long timeStepMatched, window: new VerificationWindow(2, 2));

                // Transformando a mensagem em um objeto JSON
                if (!isValido)
                    return Unauthorized(new { mensagem = "Código MFA inválido." });
            }

            // 3. Gera o Token Principal
            var token = CriarTokenJwt(usuario);

            // 4. Gera o Token de Confiança (7 dias)
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

        [Authorize]
        [HttpPost("mfa/gerar")]
        public async Task<IActionResult> GerarChaveMfa()
        {
            // 1. Pega o ID do usuário através do token JWT
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimId == null) return Unauthorized();
            var usuarioId = int.Parse(claimId);

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null) return NotFound("Usuário não encontrado.");

            // 2. Se já tiver MFA ativo, não pode gerar outro
            if (usuario.IsMfaEnabled)
                return BadRequest("MFA já está ativado para esta conta.");
            
            // 3. Gera a chave secreta em Base 32
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(secretKey);

            // 4. Salva no banco
            usuario.TotpSecret = base32Secret;
            await _context.SaveChangesAsync();

            // 5. Monta a URI que será convertida em QR Code pelo front-end
            var issuer = "FitPA";
            var uri = $"otpauth://totp/{issuer}:{usuario.Username}?secret={base32Secret}&issuer={issuer}";

            return Ok(new
            {
                mensagem = "Chave gerada. Escaneie o QR Code no app Authenticator.",
                qrCodeUri = uri,
                chaveManual = base32Secret
            });
        }

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

            // Usa a biblioteca OtpNet para validar os 6 digitos contra a chave secreta
            var totp = new Totp(Base32Encoding.ToBytes(usuario.TotpSecret));

            // Window 2, dando uma tolerância de +/- 1 minuto para relógios dessincronizados
            bool isValido = totp.VerifyTotp(request.Codigo, out long timeStepMatched, window: new VerificationWindow(2, 2));

            if (!isValido) return BadRequest("Código MFA inválido.");

            usuario.IsMfaEnabled = true;
            await _context.SaveChangesAsync();

            return Ok(new { mensagem = "MFA ativado com sucesso." });
        }

        private bool IsTrustTokenValido(string token, string username)
        {
            try
            {
               var tokenHandler = new JwtSecurityTokenHandler();
               var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

               tokenHandler.ValidateToken(token, new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(key),
                     ValidateIssuer = false, // Ignorado para simplificação
                     ValidateAudience = false,
                     ClockSkew = TimeSpan.Zero
               }, out SecurityToken validatedToken);
               
               var jwtToken = (JwtSecurityToken)validatedToken;
               var tokenUsername = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
               var tokenType = jwtToken.Claims.First(x => x.Type == "TokenType").Value;

               return tokenUsername == username && tokenType == "MfaTrust" ;
            } catch
            {
                return false;
            }
        }
    }
}