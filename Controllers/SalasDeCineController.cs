using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using WebPeliculas.Controllers.Entidades;
using WebPeliculas.DTOs;

namespace WebPeliculas.Controllers
{
    [ApiController]
    [Route("api/SalasDeCine")]
    public class SalasDeCineController : CustomBaseController
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        public SalasDeCineController(AplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineDTO>>> Get()
        {
            return await Get<SalaDeCine, SalaDeCineDTO>();
        }

        [HttpGet("{id:int}" , Name = "obtenerSalaDeCine")]
        public async Task<ActionResult<SalaDeCineDTO>> Get(int id)
        {
            return await GetById<SalaDeCine , SalaDeCineDTO>(id);  
        }

        [HttpGet("cercanos")]
        public async Task<ActionResult<List<SalaDeCineCercanoDTO>>> GetCercanos( [FromQuery] SalaDeCineCercanoFiltroDTO filtro)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var ubicacionUsuario = geometryFactory.CreatePoint(new Coordinate(filtro.Longitud, filtro.Latitud));

            var salasDeCineCercanos = await context.SalaDeCines.OrderBy(x => x.Ubicacion.Distance(ubicacionUsuario))
                                                         .Where(x => x.Ubicacion.IsWithinDistance(ubicacionUsuario, filtro.DistanciaEnMks * 1000))
                                                         .Select(x => new SalaDeCineCercanoDTO
                                                         {
                                                             Id = x.Id,
                                                             Nombre = x.Nombre,
                                                             latitud = x.Ubicacion.Y,
                                                             longitud = x.Ubicacion.X,
                                                             DistanciaEnMetros = Math.Round(x.Ubicacion.Distance(ubicacionUsuario))
                                                         })
                                                         .ToListAsync();

            return salasDeCineCercanos;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {
            return await Post<SalaDeCineCreacionDTO, SalaDeCine, SalaDeCineDTO>(salaDeCineCreacionDTO, "obtenerSalaDeCine");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] SalaDeCineCreacionDTO salaDeCineCreacionDTO )
        {
            return await Put<SalaDeCineCreacionDTO, SalaDeCine>(salaDeCineCreacionDTO, id);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }
    }

}

