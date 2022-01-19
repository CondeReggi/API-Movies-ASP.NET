using System.ComponentModel.DataAnnotations;

namespace WebPeliculas.Validaciones
{
    public class PesoArchivoValidacion : ValidationAttribute
    {
        private readonly int PesoMaximoEnMb;
        public PesoArchivoValidacion(int pesoMaximoEnMb) {
            PesoMaximoEnMb = pesoMaximoEnMb;
        }

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;    
            }

            IFormFile fromfile = value as IFormFile;

            if (fromfile == null)
            {
                return ValidationResult.Success;
            }

            if (fromfile.Length > PesoMaximoEnMb * 1024 * 1024)
            {
                return new ValidationResult($"El peso del archivo no puede exceder de {PesoMaximoEnMb}");
            }

            return ValidationResult.Success;
        }

    }
}
