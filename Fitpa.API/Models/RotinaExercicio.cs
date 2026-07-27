namespace Fitpa.API.Models
{
    public class RotinaExercicio
    {
        public int RotinaId { get; set; }
        public Rotina Rotina { get; set; } = null!;

        public int ExercicioId { get; set; }
        public Exercicio Exercicio { get; set; } = null!;

        public int Series { get; set; }
        public int Ordem { get; set; }
    }
}