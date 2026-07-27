namespace Fitpa.API.Models
{
    public class Rotina
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        // Relacionamento com o Usuário dono da Rotina
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public ICollection<Exercicio> Exercicios { get; set; } = new List<Exercicio>();
    }
}