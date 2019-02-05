using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Mozart.Composition.AspNetCore.Mvc.Results.Abstractions;
using Mozart.Composition.Core.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Results
{
    public class EntireResultHandler<T> : EntireResultHandlerBase<T> where T : class
    {
        private readonly ICachedServiceResolver<Type, IComposeModel> _modelCompositionCachedServiceResolver;

        public EntireResultHandler(ICachedServiceResolver<Type, IComposeModel> modelCompositionCachedServiceResolver)
        {
            // The custom resolver that will get our view model composition services by property type
            _modelCompositionCachedServiceResolver = modelCompositionCachedServiceResolver;
        }

        public override async Task<(T Model, int StatusCode)> HandleOfTAsync(HttpContext context)
        {
            // Leverage the IComposeModel implementation to create an entire result
            if (!_modelCompositionCachedServiceResolver.TryResolve(typeof(T), out var composeModel))
            {
                return (null, StatusCodes.Status404NotFound);
            }

            var result = (T) await composeModel.ComposeAsync(context.GetRouteData().Values);
            return (result, StatusCodes.Status200OK);
        }
    }
}
