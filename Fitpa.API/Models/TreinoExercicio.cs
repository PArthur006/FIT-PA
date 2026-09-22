namespace Fitpa.API.Models
{
    public class TreinoExercicio
    {
        public int Id { get; set; }
        public int TreinoId { get; set; }
        public Treino Treino { get; set; }
        public int ExercicioId { get; set; }
        public Exercicio Exercicio { get; set; }
        public int Ordem { get; set; }
        public ICollection<TreinoSerie> Series { get; set; } = new List<TreinoSerie>();
    }
}