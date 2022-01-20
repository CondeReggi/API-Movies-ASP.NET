using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPeliculas.Controllers.Entidades;
using WebPeliculas.DTOs;
using WebPeliculas.Helpers;
using WebPeliculas.Servicios;
using System.Linq.Dynamic.Core;

namespace WebPeliculas.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<PeliculasController> logger;
        private readonly string contenedor = "peliculas";

        public PeliculasController(AplicationDbContext context , IMapper mapper ,  IAlmacenadorArchivos almacenadorArchivos, ILogger<PeliculasController> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<PeliculaDTO>>> Get()
        {
            var peliculas = await context.Peliculas.ToListAsync();
            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("{id}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculaDetallesDTO>> GetPelicula(int id)
        {
            var pelicula = await context.Peliculas
                            .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
                            .Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
                            .FirstOrDefaultAsync(x => x.Id == id);

            if ( pelicula == null)
            {
                return NotFound();  // 404 Not found
            }

            pelicula.PeliculasActores = pelicula.PeliculasActores.OrderBy(x => x.Orden).ToList();

            return mapper.Map<PeliculaDetallesDTO>(pelicula);
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculasDTO filtroPeliculasDTO)
        {
            //Utilizaremos ejecucion DIFERIDA
            var peliculasQueryable = context.Peliculas.AsQueryable();
            if (!string.IsNullOrEmpty(filtroPeliculasDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculasDTO.Titulo));
            }

            if (filtroPeliculasDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
            }
            else
            {
                peliculasQueryable = peliculasQueryable.Where(x => !x.EnCines);
            }

            if (filtroPeliculasDTO.ProximosEstrenos)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > DateTime.Today);
            }

            if (filtroPeliculasDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                                                        .Contains(filtroPeliculasDTO.GeneroId));
            }


            //Esta forma es valida, pero se debe crear una seccion asi para cada atributo por el que se deseea ordenar de forma asc o desc
            //En otra alterminativa se puede instalar el PAGKAGE = System.Linq.Dynamic.Core

            //if (!string.IsNullOrEmpty(filtroPeliculasDTO.campoOrdenar))
            //{
            //    if (filtroPeliculasDTO.campoOrdenar == "titulo")
            //    {
            //        if (filtroPeliculasDTO.OrdenAsc)
            //        {
            //            peliculasQueryable = peliculasQueryable.OrderBy(x => x.Titulo);
            //        }
            //        else
            //        {
            //            peliculasQueryable = peliculasQueryable.OrderByDescending(x => x.Titulo);
            //        }
            //    } 
            //}

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.campoOrdenar))
            {
                //peliculasQueryable = peliculasQueryable.OrderBy("titulo ascending"); EJEMPLO

                var esAscendente = filtroPeliculasDTO.OrdenAsc ? "ascending" : "descending";

                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculasDTO.campoOrdenar} {esAscendente}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                    return NotFound("Algo ha fallado debido a: " + ex.Message);
                }
            }

            await HttpContext.InsertarParametrosPaginacion( peliculasQueryable, filtroPeliculasDTO.CantidadRegistrosPorPagina );
            var peliculas = await peliculasQueryable.Paginar(filtroPeliculasDTO.Paginacion).ToListAsync();

            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }
        

        [HttpPost]

        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO) 
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);

            //return Ok();

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);

                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, peliculaCreacionDTO.Poster.ContentType);
                }
            }

            context.Add(pelicula);
            await context.SaveChangesAsync();

            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);

            //return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, dto);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, peliculaDTO);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var peliculaDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (peliculaDB == null)
            {
                return NotFound();
            }

            peliculaDB = mapper.Map(peliculaCreacionDTO, peliculaDB);

            if ( peliculaCreacionDTO.Poster != null)
            {
                using (var memoryString = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryString);
                    var contenido = memoryString.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, peliculaDB.Poster, peliculaCreacionDTO.Poster.ContentType);

                }
            }

            await context.SaveChangesAsync();
            return NoContent(); 
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entidadDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (entidadDB == null)
            {
                return NotFound();
            }

            var entidadDTO = mapper.Map<PeliculaPatchDTO>(entidadDB);
            patchDocument.ApplyTo(entidadDTO, ModelState);

            var esValido = TryValidateModel(entidadDTO);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(entidadDTO, entidadDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Peliculas.AnyAsync(x =>x.Id == id); 

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Pelicula() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();

        }
             
    }
}
