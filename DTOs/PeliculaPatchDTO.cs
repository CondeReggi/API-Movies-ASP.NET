using System.ComponentModel.DataAnnotations;

namespace WebPeliculas.DTOs
{
    public class PeliculaPatchDTO
    {
        [Required]
        [StringLength(120)]
        public string Titulo { get; set; }
        public bool EnCines { get; set; }
        public DateTime FechaEstreno { get; set; }
    }
}
