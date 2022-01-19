using System.ComponentModel.DataAnnotations;

namespace WebPeliculas.Validaciones
{
    public class TipoDeArchivoValidacion : ValidationAttribute
    {
        private readonly string[] tiposValidos;

        public TipoDeArchivoValidacion( string[] tiposValidos)
        {
            this.tiposValidos = tiposValidos;
        }

        public TipoDeArchivoValidacion(GrupoTipoArchivo grupoTipoArchivo)
        {
            if (grupoTipoArchivo == GrupoTipoArchivo.Imagen)
            {
                tiposValidos = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ( value == null) {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;    

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if(!tiposValidos.Contains(formFile.ContentType))
            {
                return new ValidationResult($"El tipo de archivo debe ser uno de los siguientes: { string.Join(" - ", tiposValidos) }");
            }

            return ValidationResult.Success;    
        }
    }
}
