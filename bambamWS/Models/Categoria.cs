using System.ComponentModel.DataAnnotations;

namespace bambamWS.Models
{
    public class Categoria
    {
        [Key]
        public int catId { get; set; }
        public string catNombre { get; set; }
        public int catEstado { get; set; }
    }
}
