using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Mozart.Composition.AspNetCore.Mvc.Results.Abstractions;
using Mozart.Composition.Core.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Results
{
    public class CompositionResultHandler<TModel> : CompositionResultHandlerBase<TModel> where TModel : class, new()
    {
        private readonly IMozartModelComposer<TModel> _modelComposer;

        public CompositionResultHandler(IMozartModelComposer<TModel> modelComposer)
        {
            // The custom composer that will orchestrate the composition
            _modelComposer = modelComposer ?? throw new ArgumentNullException(nameof(modelComposer));
        }

        public override async Task<(TModel Model, int StatusCode)> HandleOfTAsync(HttpContext context)
        {
            // This method simply wraps the behaviour of IMozartModelComposer and enhances with status codes for ASP.NET
            var result = await _modelComposer.BuildCompositeModelAsync(context.GetRouteData().Values);
            var statusCode = result != null ? StatusCodes.Status200OK : StatusCodes.Status404NotFound;
            return (result, statusCode);
        }
    }
}
