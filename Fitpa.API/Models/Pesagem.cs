using System.Text.Json.Serialization;

namespace Fitpa.API.Models
{
    /*
     * Entidade de pesagem
     * Representa um registro único armazenado no banco.
     */
    public class Pesagem
    {
        public int Id { get; set; }
        public DateOnly Data { get; set; }
        public decimal Peso { get; set; }

        // Relacionamento com o usuário
        public int UsuarioId { get; set; }
        [JsonIgnore] // Evita referência circular durante a serialização
        public Usuario? Usuario { get; set; }
    }
}