using System.ComponentModel.DataAnnotations;
using WebPeliculas.Validaciones;

namespace WebPeliculas.DTOs
{
    public class ActorCreacionDTO : ActorPatchDTO
    {
        
        [PesoArchivoValidacion(pesoMaximoEnMb: 4)]
        [TipoDeArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile? Foto { get; set; }
    }
}
