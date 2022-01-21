using System.ComponentModel.DataAnnotations;

namespace WebPeliculas.DTOs
{
    public class SalaDeCineCreacionDTO
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }

        [Range(-90,90)]
        public double latitud { get; set; }
        [Range(-180, 180)]
        public double longitud { get; set; }
    }
}
