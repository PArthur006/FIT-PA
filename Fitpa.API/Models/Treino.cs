namespace Fitpa.API.Models
{
    public class Treino
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Data { get; set; }
        public ICollection<TreinoExercicio> ExerciciosPraticados { get; set; } = new List<TreinoExercicio>();
    }
}