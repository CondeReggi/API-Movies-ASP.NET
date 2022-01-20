using Microsoft.EntityFrameworkCore;

namespace WebPeliculas.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacion<T>(this HttpContext httpContext, IQueryable<T> queryable, int cantidadRegistrosPorPagina)
        {
            double cantidad = await queryable.CountAsync(); //Cuenta los queryable
            double cantidadPaginas = Math.Ceiling(cantidad / cantidadRegistrosPorPagina); //Toma la division y la "redondea"

            httpContext.Response.Headers.Add("cantidadPaginas" , cantidadPaginas.ToString()); //Agrega al link un cantidadDePaginas + la cantidad averiguada
        }
    }
}
