using Microsoft.EntityFrameworkCore;
using WebPeliculas.Entidades;
using WebPeliculas.Controllers.Entidades;

namespace WebPeliculas
{
    public class AplicationDbContext : DbContext 
    {
        public AplicationDbContext( DbContextOptions options ) : base(options)
        {

        }

        public DbSet<Genero> Generos { get; set; } // CREATE TABLE GENEROS 
        public DbSet<Actor> Actores { get; set; } // CREATE TABLE ACTORES
    }

}
