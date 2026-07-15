using Microsoft.EntityFrameworkCore;
using Fitpa.API.Data;

// Usings para autenticator
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/*
 * Serviços da aplicação
 * Registra OpenAPI, CORS, banco de dados e controllers.
 */
builder.Services.AddOpenApi();

/*
 * Política de CORS
 * Permite chamadas do front-end local durante o desenvolvimento.
 */
builder.Services.AddCors(options =>
{
   options.AddPolicy("PermitirFrontEnd", policy =>
   {
       policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
   });
});

/*
 * Persistência
 * Configura o DbContext com a conexão PostgreSQL definida em appsettings.
 */
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

/*
* Autenticação JWT
* Configura o middleware para validar tokens JWT nas requisições.    
*/
var jwtKey = builder.Configuration["Jwt:Key"];
var keyBytes = Encoding.ASCII.GetBytes(jwtKey!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Preciso mudar para true em produção
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Sem tolerância de tempo para expiração
    };
});

/*
 * API MVC
 * Habilita o suporte aos controllers da aplicação.
 */
builder.Services.AddControllers();

var app = builder.Build();

/*
 * Pipeline HTTP
 * Aplica CORS antes do roteamento dos endpoints.
 */
app.UseCors("PermitirFrontEnd");

/*
 * Middleware de autenticação
 */
app.UseAuthentication();
app.UseAuthorization();

/*
 * Ambiente de desenvolvimento
 * Expõe a documentação OpenAPI apenas em dev.
 */
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

/*
 * Rotas da API
 * Mapeia os controllers para receber as requisições HTTP.
 */
app.MapControllers();

app.Run();
