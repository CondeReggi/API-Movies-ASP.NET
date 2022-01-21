using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPeliculas.Controllers.Entidades;

namespace WebPeliculas.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(AplicationDbContext context , IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        // GET Generico
        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class
        {
            var entidades = await context.Set<TEntidad>().AsNoTracking().ToListAsync();

            //AsNoTracking => No trakea => performs no additional processing or storage of the entities which are returned by the query.
            //However, it also means that you can't update these entities without reattaching them to the tracking graph.

            var dtos = mapper.Map<List<TDTO>>(entidades);
            return dtos;
        }

        protected async Task<ActionResult<TDTO>> GetById<TEntidad, TDTO>(int id) where TEntidad : class, IId
        {
            // Aca tenemos un problema, no sabemos si el TEntidad que nos viene posee un campo Id,
            // Para esto utilizamos tecnicas como crear una interfaz para saber si trae un campo Id

            var entidad = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            return mapper.Map<TDTO>(entidad);
        }

        protected async Task<ActionResult> Post<TCreacion, TEntidad, TLectura>(TCreacion creacionDTO, string nombreRuta) where TEntidad: class, IId
        {
            try
            {
                var entidad = mapper.Map<TEntidad>(creacionDTO);
                context.Add(entidad);

                await context.SaveChangesAsync();
                var dtoLectura = mapper.Map<TLectura>(entidad);

                return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, dtoLectura);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        protected async Task<ActionResult> Put<TCreacion, TEntidad>(TCreacion creacionDTO, int id) where TEntidad : class, IId
        {
            try
            {
                var entidad = mapper.Map<TEntidad>(creacionDTO); // Voy a mapear el generoCreacionDTO a Genero (Id, Nombre)
                entidad.Id = id; // Igualo la id a la que nosotros recibimos

                context.Entry(entidad).State = EntityState.Modified; //Modifico todas las entradas de la tabla generando un codigo SQL como este

                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        protected async Task<ActionResult> Delete<TEntidad>(int id) where TEntidad : class, IId, new()
        {
            var existe = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound(); // StatusCode(StatusCodes.Status400BadRequest); // Se obtiene lo mismo con Esto o BadRequest escrito asi xd
            }

            context.Remove(new TEntidad() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
