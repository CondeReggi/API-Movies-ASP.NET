﻿using System.ComponentModel.DataAnnotations;

namespace WebPeliculas.Controllers.DTOs
{
    public class AutorDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Nombre { get; set; }
    }
}
