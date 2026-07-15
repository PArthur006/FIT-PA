using Fitpa.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Fitpa.API.Data
{
    /*
     * Contexto principal do banco
     * Centraliza o acesso ao Entity Framework para as entidades da aplicação.
     */
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        /*
         * Tabela de pesagens
         * Representa o conjunto de registros persistidos no banco.
         */
        public DbSet<Pesagem> Pesagens { get; set; }
    }
}