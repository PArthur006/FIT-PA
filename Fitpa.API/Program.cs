using Microsoft.EntityFrameworkCore;
using Fitpa.API.Data;

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
