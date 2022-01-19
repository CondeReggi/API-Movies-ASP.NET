using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WebPeliculas.Servicios
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {
        private readonly string connectionString;
        public AlmacenadorArchivosAzure(IConfiguration configurations)
        {
            connectionString = configurations.GetConnectionString("AzureStorageConnection"); //Obtengo la connectionString de Azure Storage
        }
        public async Task BorrarArchivo(string ruta, string contenedor)
        {
            if( string.IsNullOrEmpty(ruta)) // No hay una imagen ya existente
            {
                return;
            }

            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();

            var archivo = Path.GetFileName(ruta);
            var blob = cliente.GetBlobClient(archivo);

            await blob.DeleteIfExistsAsync();  
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType)
        {
            await BorrarArchivo(ruta, contenedor);
            return await GuardarArchivo(contenido, extension, contenedor, contentType);

        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType)
        {
            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            cliente.SetAccessPolicy(PublicAccessType.Blob);

            var archivadoNombre = $"{Guid.NewGuid()}{extension}";
            var blob = cliente.GetBlobClient(archivadoNombre);

            var blobUploadOptions = new BlobUploadOptions();
            var blobHttpHeader = new BlobHttpHeaders();

            blobHttpHeader.ContentType = contentType;
            blobUploadOptions.HttpHeaders = blobHttpHeader;

            await blob.UploadAsync(new BinaryData(contenido), blobUploadOptions);
            return blob.Uri.ToString();

        }
    }
}
