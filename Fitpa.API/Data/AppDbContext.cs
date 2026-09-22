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

        public DbSet<Exercicio> Exercicios { get; set; }
        public DbSet<Rotina> Rotinas { get; set; }
        public DbSet<RotinaExercicio> RotinasExercicios { get; set; }
        public DbSet<Treino> Treinos { get; set; }
        public DbSet<TreinoExercicio> TreinosExercicios { get; set; }
        public DbSet<TreinoSerie> TreinosSeries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Pesagem>()
                .Property(p => p.Id)
                .HasColumnName("ID");

            // Configura a relação 1:N
            modelBuilder.Entity<Pesagem>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Pesagens)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade); // Exclui pesagens ao remover usuário
            

            modelBuilder.Entity<Rotina>()
                .HasMany(r => r.Exercicios)
                .WithMany(e => e.Rotinas)
                .UsingEntity<RotinaExercicio>(
                    j => j.HasOne(re => re.Exercicio)
                        .WithMany()
                        .HasForeignKey(re => re.ExercicioId),
                    j => j.HasOne(re => re.Rotina)
                        .WithMany()
                        .HasForeignKey(re => re.RotinaId),
                    j =>
                    {
                        j.HasKey(re => new { re.RotinaId, re.ExercicioId });
                    });
            
            modelBuilder.Entity<TreinoExercicio>()
                .HasMany(te => te.Series)
                .WithOne(s => s.TreinoExercicio)
                .HasForeignKey(s => s.TreinoExercicioId)
                .OnDelete(DeleteBehavior.Cascade); 
            
            modelBuilder.Entity<Treino>()
                .HasMany(t => t.ExerciciosPraticados)
                .WithOne(te => te.Treino)
                .HasForeignKey(te => te.TreinoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}