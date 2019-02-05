using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozart.Composition.AspNetCore.Mvc.Results.Abstractions
{
    public abstract class CompositionResultHandlerBase<T> : IHandleResult<T> where T : class, new()
    {
        public async Task<(object Model, int StatusCode)> HandleAsync(HttpContext context)
        {
            return await HandleOfTAsync(context);
        }

        public abstract Task<(T Model, int StatusCode)> HandleOfTAsync(HttpContext context);
    }
}
