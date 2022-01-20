using WebPeliculas.Controllers.DTOs;
using WebPeliculas.Controllers.Entidades;
using WebPeliculas.DTOs;
using WebPeliculas.Entidades;
using AutoMapper;

namespace WebPeliculas.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() // Constructor de la clase
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();
            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();

            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActores));
            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();

            CreateMap<Pelicula, PeliculaDetallesDTO>()
                .ForMember(x => x.Generos , options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));   

        }
        private List<ActorPeliculaDetalleDTO> MapPeliculasActores( Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<ActorPeliculaDetalleDTO>();

            if ( pelicula.PeliculasActores == null)
            {
                return resultado;
            }

            foreach (var actorPelicula in pelicula.PeliculasActores)
            {
                resultado.Add(new ActorPeliculaDetalleDTO() { ActorId = actorPelicula.ActorId, Personaje = actorPelicula.Personaje , NombrePersona = actorPelicula.Actor.Nombre});
            }

            return resultado;   
        }

        private List<GeneroDTO> MapPeliculasGeneros(Pelicula pelicula, PeliculaDetallesDTO peliculaDetalleDTO)
        {
            var restultado = new List<GeneroDTO>();
            if(pelicula.PeliculasGeneros == null)
            {
                return restultado;
            }
            foreach(var generoPelicula in pelicula.PeliculasGeneros)
            {
                restultado.Add(new GeneroDTO() { Id = generoPelicula.GeneroId, Nombre = generoPelicula.Genero.Nombre });
            }

            return restultado;
        }

        private List<PeliculasGeneros> MapPeliculasGeneros( PeliculaCreacionDTO peliculasCreacionDTO, Pelicula pelicula)
        {
            var resultado =  new List<PeliculasGeneros>();
            if ( peliculasCreacionDTO.GenerosIds == null)
            {
                return resultado;
            }

            foreach( var id in peliculasCreacionDTO.GenerosIds)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });    
            }

            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculasCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();

            if (peliculasCreacionDTO.Actores == null)
            {
                return resultado;
            }

            foreach (var actor in peliculasCreacionDTO.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.ActorId, Personaje = actor.Personaje });
            }

            return resultado;
        }
    }
}
