using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace WebPeliculas.Helpers
{
    public class TypeBinder<T>  : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var nombrePorpiedad = bindingContext.ModelName;
            var proveedorDeValores = bindingContext.ValueProvider.GetValue(nombrePorpiedad);

            if ( proveedorDeValores == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            try
            {
                var valorDeserializado = JsonConvert.DeserializeObject<T>(proveedorDeValores.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(valorDeserializado);

            }catch (Exception)
            {
                bindingContext.ModelState.TryAddModelError(nombrePorpiedad, "Valor invalido para tipo List<int>");
            }

            return Task.CompletedTask;
        }
    }
}
