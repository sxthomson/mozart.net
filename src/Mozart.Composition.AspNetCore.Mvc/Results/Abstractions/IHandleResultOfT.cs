using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozart.Composition.AspNetCore.Mvc.Results.Abstractions
{
    public interface IHandleResult<T> : IHandleResult where T : class
    {
        Task<(T ViewModel, int StatusCode)> HandleOfT(HttpContext context);
    }
}