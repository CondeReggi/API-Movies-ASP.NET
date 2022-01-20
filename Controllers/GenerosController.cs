using WebPeliculas.Controllers.DTOs;
using WebPeliculas.DTOs;
using WebPeliculas.Entidades;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebPeliculas.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController: ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(AplicationDbContext context , IMapper mapper )
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]   
        public async Task<ActionResult<List<GeneroDTO>>> Get()
        {
            //return await context.Generos.ToListAsync();
            var entidades = await context.Generos.ToListAsync(); // Capturo normalmente los datos
            var dtos = mapper.Map<List<GeneroDTO>>( entidades ); // Los mapeo a GenerosDTO todas las entidades (es decir los reales)
            return dtos; // Devuelvo los DTO
        }

        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            var existeDato = await context.Generos.FirstOrDefaultAsync( x => x.Id == id );

            if (existeDato == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            var dto = mapper.Map<GeneroDTO>(existeDato);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO )
        {
            try
            {
                var entidad = mapper.Map<Genero>(generoCreacionDTO);
                context.Add(entidad);

                await context.SaveChangesAsync();
                var generoDTO = mapper.Map<GeneroDTO>(entidad);

                return new CreatedAtRouteResult("obtenerGenero", new { id = generoDTO.Id }, generoDTO);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);  
            }

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put( int id , [FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            try
            {
                var entidad = mapper.Map<Genero>(generoCreacionDTO); // Voy a mapear el generoCreacionDTO a Genero (Id, Nombre)
                entidad.Id = id; // Igualo la id a la que nosotros recibimos
                 
                context.Entry(entidad).State = EntityState.Modified; //Modifico todas las entradas de la tabla generando un codigo SQL como este

                //UPDATE person
                //SET FirstName = 'whatever first name is',
                //    LastName = 'whatever last name is'
                //WHERE Id = 123; --whatever Id is.

                //Si usara otra cosa en vez de Entry entidad State = Entity.Modified => usando context.Model.Attach(entity); 
                                                                                        // Entity.Propiety = "Value to change";
                                                                                        //context.SaveChanges();

                //Generaria una sentencia SQL Como esta: => 
                //UPDATE person
                //SET FirstName = 'John'
                //WHERE Id = 123; --whatever Id is.


                await context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Generos.AnyAsync(x => x.Id == id); 

            if (!existe)
            {
                return StatusCode(StatusCodes.Status400BadRequest); // Se obtiene lo mismo con Esto o BadRequest escrito asi xd
            }

            context.Remove(new Genero() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }


    }
}
