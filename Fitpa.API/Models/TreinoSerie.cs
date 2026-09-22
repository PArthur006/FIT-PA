namespace Fitpa.API.Models
{
    public class TreinoSerie
    {
        public int Id { get; set; }
        public int TreinoExercicioId { get; set; }
        public TreinoExercicio TreinoExercicio { get; set; }
        public int Repeticoes { get; set; }
        public double Peso { get; set; }
    }
}