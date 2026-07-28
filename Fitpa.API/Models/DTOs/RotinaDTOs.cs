namespace Fitpa.API.Models.DTOs
{
    public class RotinaCreateDto
    {
        public string Nome { get; set; } = string.Empty;
        public List<RotinaExercicioDto> Exercicios { get; set; } = new List<RotinaExercicioDto>();
    }

    public class RotinaExercicioDto
    {
        public int ExercicioId {get; set; }
        public int Series { get; set; }
        public int Ordem { get; set; }
    }

    public class RotinaResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public List<RotinaExercicioResponseDto> Exercicios { get; set; } = new List<RotinaExercicioResponseDto>();
    }

    public class RotinaExercicioResponseDto
    {
        public int ExercicioId {get; set; }
        public string NomeExercicio { get; set; } = string.Empty;
        public string GrupoMuscular { get; set; } = string.Empty;
        public int Series { get; set; }
        public int Ordem { get; set; }
    }
}