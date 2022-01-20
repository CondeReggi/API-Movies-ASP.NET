using Microsoft.AspNetCore.Mvc;
using WebPeliculas.Helpers;
using WebPeliculas.Validaciones;

namespace WebPeliculas.DTOs
{
    public class PeliculaCreacionDTO : PeliculaPatchDTO
    {
        
        [PesoArchivoValidacion( pesoMaximoEnMb: 4)]
        [TipoDeArchivoValidacion(GrupoTipoArchivo.Imagen)] // Porque es un enum
        public IFormFile? Poster { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIds { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculaCreacionDTO>>))]
        public List<ActorPeliculaCreacionDTO> Actores { get; set; } 
    }
}
