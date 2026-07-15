namespace Fitpa.API.Models
{
    /*
     * Entidade de pesagem
     * Representa um registro único armazenado no banco.
     */
    public class Pesagem
    {
        public int ID { get; set; }
        public DateOnly Data { get; set; }
        public decimal Peso { get; set; }
    }
}