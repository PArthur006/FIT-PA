namespace Fitpa.API.Models.DTOs
{
    public class TreinoCreateDto
    {
        public DateTime Data { get; set; }
        public List<TreinoExercicioCreateDto> ExerciciosExecutados { get; set; }
    }

    public class TreinoExercicioCreateDto
    {
        public int ExercicioId { get; set; }
        public List<TreinoSerieCreateDto> Series { get; set; } = new();
    }

    public class TreinoSerieCreateDto
    {
        public int Repeticoes { get; set; }
        public double Peso { get; set; }
    }
}