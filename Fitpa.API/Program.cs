using Microsoft.EntityFrameworkCore;
using Fitpa.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
   options.AddPolicy("PermitirFrontEnd", policy =>
   {
       policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
   });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Acrescenta o suporte para controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configura o CORS 
app.UseCors("PermitirFrontEnd");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Mapeiamento dos controllers
app.MapControllers();

app.Run();
