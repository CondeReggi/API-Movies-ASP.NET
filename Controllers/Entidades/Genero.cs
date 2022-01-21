using System.ComponentModel.DataAnnotations;
using WebPeliculas.Controllers.Entidades;

namespace WebPeliculas.Entidades
{
    public class Genero : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }
        public List<PeliculasGeneros> PeliculasGeneros { get; set; }   
    } 
}
