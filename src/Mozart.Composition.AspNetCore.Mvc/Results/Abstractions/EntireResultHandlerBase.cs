using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozart.Composition.AspNetCore.Mvc.Results.Abstractions
{
    public abstract class EntireResultHandlerBase<T> : IHandleResult<T> where T : class
    {
        public Task<(object ViewModel, int StatusCode)> Handle(HttpContext context)
        {
            return HandleOfT(context).ContinueWith(t => ((object)t.Result.ViewModel, t.Result.StatusCode));
        }

        public abstract Task<(T ViewModel, int StatusCode)> HandleOfT(HttpContext context);
    }
}
