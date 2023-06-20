using System.ComponentModel.DataAnnotations;

namespace bambamWS.Models
{
    public class Usurol
    {
        [Key]
        public int urId { get; set; } = 0;
        public string urUsu { get; set; } = "";
        public int urRol { get; set; } = 0;
    }
}
