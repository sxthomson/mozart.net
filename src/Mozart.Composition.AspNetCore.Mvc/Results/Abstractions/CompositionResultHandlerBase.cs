using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozart.Composition.AspNetCore.Mvc.Results.Abstractions
{
    public abstract class CompositionResultHandlerBase<T> : IHandleResult<T> where T : class, new()
    {
        public async Task<(object ViewModel, int StatusCode)> Handle(HttpContext context)
        {
            return await HandleOfT(context);
        }

        public abstract Task<(T ViewModel, int StatusCode)> HandleOfT(HttpContext context);
    }
}
