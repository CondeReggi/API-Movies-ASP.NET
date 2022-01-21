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

        protected override void OnModelCreating(ModelBuilder mBuilder)
        {
            mBuilder.Entity<PeliculasActores>()
                .HasKey(x => new { x.ActorId, x.PeliculaId });

            mBuilder.Entity<PeliculasGeneros>()
                .HasKey(x => new { x.GeneroId, x.PeliculaId });

            mBuilder.Entity<PeliculasSalasDeCine>().HasKey(e => new { e.PeliculaId, e.SalaDeCineId });

            base.OnModelCreating(mBuilder);
        }

        public DbSet<Genero> Generos { get; set; } // CREATE TABLE GENEROS 
        public DbSet<Actor> Actores { get; set; } // CREATE TABLE ACTORES
        public DbSet<Pelicula> Peliculas { get; set;} // CREATE TABLE PELICULAS
        public DbSet<PeliculasActores> PeliculasActores { get;set; }
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }

        public DbSet<SalaDeCine> SalaDeCines { get; set; }
        public DbSet<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }

    }

}
