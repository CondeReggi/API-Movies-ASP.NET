using WebPeliculas.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPeliculas.Controllers.DTOs;
using WebPeliculas.Controllers.Entidades;
using WebPeliculas.Servicios;
using Microsoft.AspNetCore.JsonPatch;
using WebPeliculas.Helpers;

namespace WebPeliculas.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresControllers : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";

        public ActoresControllers(AplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO )
        {
            try
            {
                var queryable = context.Actores.AsQueryable();
                
                await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistrosPorPagina);

                // Hasta este momento hemos agregado la cantidad de paginas a la cabecera del Http
                // Hay que agregarla a nivel de EntityFramework

                var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
                return mapper.Map<List<ActorDTO>>(entidades);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var existeDato = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (existeDato == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            var dto = mapper.Map<ActorDTO>(existeDato);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            try
            {
                var entidad = mapper.Map<Actor>(actorCreacionDTO); // Se supone que aca pone NULL el valor de Foto

                if( actorCreacionDTO.Foto != null )
                {
                    using ( var memoryStream = new MemoryStream())
                    {
                        await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                        var contenido = memoryStream.ToArray();
                        var extension  = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                        entidad.Foto = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, actorCreacionDTO.Foto.ContentType);
                    }
                }

                context.Add(entidad);

                //await context.SaveChangesAsync();
                var dto = mapper.Map<ActorDTO>(entidad);
                return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, dto);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            try
            {
                var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
                if (actorDB == null) {  return NotFound(); }

                actorDB = mapper.Map(actorCreacionDTO, actorDB);

                if (actorCreacionDTO.Foto != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                        var contenido = memoryStream.ToArray();
                        var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                        actorDB.Foto = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, actorDB.Foto, actorCreacionDTO.Foto.ContentType);
                    }
                }

                //var entidad = mapper.Map<Actor>(actorCreacionDTO);
                //entidad.Id = id;

                //context.Entry(entidad).State = EntityState.Modified;
                await context.SaveChangesAsync();

                //context.Actores.Attach(entidad); // State = Unchanged
                //entidad.Id = id; // State = Modified, and only the FirstName property is dirty.
                //await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }

        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id , [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entidadDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (entidadDB == null)
            {
                return NotFound();
            }

            var entidadDTO = mapper.Map<ActorPatchDTO>(entidadDB);
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
            var existe = await context.Actores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return StatusCode(StatusCodes.Status400BadRequest); // Se obtiene lo mismo con Esto o BadRequest escrito asi xd
            }

            context.Remove(new Actor() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }


    }
}
