using Fitpa.API.Models;

namespace Fitpa.API.Data
{
    public static class DbSeeder
    {
        public static void SeedExercicios(AppDbContext context)
        {
            // Verifica se a tabela já possui algum registro
            if (!context.Exercicios.Any())
            {
                var exerciciosIniciais = new List<Exercicio>
                {
                    new Exercicio { Nome = "Supino Reto com Barra", GrupoMuscular = "Peito" },
                    new Exercicio { Nome = "Supino Inclinado com Halteres", GrupoMuscular = "Peito" },
                    new Exercicio { Nome = "Agachamento Livre", GrupoMuscular = "Pernas" },
                    new Exercicio { Nome = "Leg Press 45º", GrupoMuscular = "Pernas" },
                    new Exercicio { Nome = "Puxada Frontal", GrupoMuscular = "Costas" },
                    new Exercicio { Nome = "Remada Curvada", GrupoMuscular = "Costas" },
                    new Exercicio { Nome = "Desenvolvimento com Halteres", GrupoMuscular = "Ombros" },
                    new Exercicio { Nome = "Elevação Lateral", GrupoMuscular = "Ombros" },
                    new Exercicio { Nome = "Rosca Direta", GrupoMuscular = "Bíceps" },
                    new Exercicio { Nome = "Tríceps Pulley", GrupoMuscular = "Tríceps" }
                };

                context.Exercicios.AddRange(exerciciosIniciais);
                context.SaveChanges();
            }
        }
    }
}