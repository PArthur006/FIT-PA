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
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pesagem>()
                .Property(p => p.Id)
                .HasColumnName("ID");

            // Configura a relação 1:N
            modelBuilder.Entity<Pesagem>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Pesagens)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade); // Exclui pesagens ao remover usuário
        }
    }
}