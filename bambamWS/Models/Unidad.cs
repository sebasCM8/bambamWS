using System.ComponentModel.DataAnnotations;

namespace bambamWS.Models
{
    public class Unidad
    {
        [Key]
        public int uniId { get; set; }
        public string uniNombre { get; set; }
        public int uniEstado { get; set; }
    }
}
