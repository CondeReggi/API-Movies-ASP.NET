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
            CreateMap<Genero, AutorDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();
            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());
        }
    }
}
