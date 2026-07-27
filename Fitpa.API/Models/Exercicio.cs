namespace Fitpa.API.Models
{
    public class Exercicio
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string GrupoMuscular { get; set; } = string.Empty;
        // Relacionamento reverso para o EF Core
        public ICollection<Rotina> Rotinas { get; set; } = new List<Rotina>();
    }
}