namespace WebPeliculas.Servicios
{
    public interface IAlmacenadorArchivos
    {
        // Un contenedor en terminos de AzureStorage es mas que una carpeta donde guardar el modelo (es simplemente un tema de organizacion)
        Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType);
        Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType);
        Task BorrarArchivo(string ruta, string contenedor);
    }
}
