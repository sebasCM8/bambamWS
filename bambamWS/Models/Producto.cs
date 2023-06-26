using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bambamWS.Models
{
    public class Producto
    {
        [Key]
        public int proId { get; set; }
        public string proNombre { get; set; }
        public string proDesc { get; set; }
        public decimal proPrecio { get; set; }
        public int proUni { get; set; }
        public int proCat { get; set; }
        public int proEstado { get; set; }

        [NotMapped]
        public string proUniNombre { get; set; } = "";
        [NotMapped]
        public string proCatNombre { get; set; } = "";
        [NotMapped]
        public decimal proStock { get; set; } = 0;
    }
}
