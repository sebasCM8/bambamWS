using System.ComponentModel.DataAnnotations;

namespace bambamWS.Models
{
    public class Ineg
    {
        [Key]
        public int ieId { get; set; } = 0;
        public int ieTipo { get; set; } = 0;
        public int iePro { get; set; } = 0;
        public decimal ieCant { get; set; } = 0;
        public DateTime ieFvto { get; set; } = DateTime.Now;
        public decimal iePrecio { get; set; } = 0;
        public int ieEstado { get; set; } = 0;
    }
}
