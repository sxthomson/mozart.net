using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Mozart.Composition.AspNetCore.Mvc.Results.Abstractions;
using Mozart.Composition.Core.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.Filters
{
    public class CompositionResultActionFilter : IAsyncActionFilter
    {
        private readonly ICachedServiceResolver<string, IHandleResult> _resultHandlerResolver;

        public CompositionResultActionFilter(ICachedServiceResolver<string, IHandleResult> resultHandlerResolver)
        {
            _resultHandlerResolver = resultHandlerResolver;        
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!HttpMethods.IsGet(context.HttpContext.Request.Method))
            {
                return;
            }

            if (resultContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                _resultHandlerResolver.TryResolve(controllerActionDescriptor.Id, out var handler);
                if (handler == null)
                {
                    resultContext.Result = new NotFoundResult();
                }
                else
                {
                    var result = await handler.HandleAsync(context.HttpContext);
                    ApplyResultToContext(resultContext, result);
                }
            }
        }

        private static void ApplyResultToContext(ActionExecutedContext context, (object Model, int StatusCode) result)
        {
            // TODO - replace with strategy pattern            
            if (context.Result is ViewResult viewResult && viewResult.ViewData.Model == null)
            {
                //MVC
                viewResult.ViewData.Model = result.Model;
                viewResult.StatusCode = result.StatusCode;
            }
            else if (context.Result is ObjectResult objectResult && objectResult.Value == null)
            {
                //WebAPI
                objectResult.Value = result.Model;
                objectResult.StatusCode = result.StatusCode;

            }
            else if (context.Result != null)
            {
                var statusCodeResult = new ObjectResult(result.Model) { StatusCode = result.StatusCode };
                context.Result = statusCodeResult;
            }
        }
    }
}
