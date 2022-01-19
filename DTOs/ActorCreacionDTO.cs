using System.ComponentModel.DataAnnotations;
using WebPeliculas.Validaciones;

namespace WebPeliculas.DTOs
{
    public class ActorCreacionDTO
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }


        [PesoArchivoValidacion(pesoMaximoEnMb: 4)]
        [TipoDeArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile? Foto { get; set; }
    }
}
