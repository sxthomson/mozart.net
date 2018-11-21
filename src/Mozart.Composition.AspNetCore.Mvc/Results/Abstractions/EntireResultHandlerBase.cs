using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozart.Composition.AspNetCore.Mvc.Results.Abstractions
{
    public abstract class EntireResultHandlerBase<T> : IHandleResult<T> where T : class
    {
        public async Task<(object Model, int StatusCode)> Handle(HttpContext context)
        {
            return await HandleOfT(context);
        }

        public abstract Task<(T Model, int StatusCode)> HandleOfT(HttpContext context);
    }
}
