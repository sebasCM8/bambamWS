using System.Text.RegularExpressions;
using System;
using System.ComponentModel.DataAnnotations;

namespace bambamWS.Models
{
    public class Usuario
    {
        [Key]
        public string usuId { get; set; }
        public string usuPass { get; set; }
        public string usuNombre { get; set; }
        public string usuApellido { get; set; }
        public string usuCI { get; set; }
        public string usuCelular { get; set; }
        public int usuEstado { get; set; }
    }
}
