namespace Fitpa.API.Models
{
    public class Pesagem
    {
        public int ID { get; set; }
        public DateOnly Data { get; set; }
        public decimal Peso { get; set; }
    }
}