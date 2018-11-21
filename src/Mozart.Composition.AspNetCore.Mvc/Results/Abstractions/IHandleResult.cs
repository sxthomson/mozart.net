using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mozart.Composition.AspNetCore.Mvc.Results.Abstractions
{
    public interface IHandleResult
    {
        Task<(object Model, int StatusCode)> Handle(HttpContext context);
    }
}