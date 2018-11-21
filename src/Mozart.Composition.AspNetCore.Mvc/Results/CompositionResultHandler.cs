using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Mozart.Composition.AspNetCore.Mvc.Results.Abstractions;
using Mozart.Composition.Core.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Results
{
    public class CompositionResultHandler<TModel> : CompositionResultHandlerBase<TModel> where TModel : class, new()
    {
        private readonly ICachedServiceResolver<Type, IComposeModel> _modelComposerResolver;
        private readonly IEnumerable<PropertyInfo> _cachedPropertyInfos;

        public CompositionResultHandler(ICachedServiceResolver<Type, IComposeModel> modelComposerResolver)
        {
            // The custom resolver that will get our view model composition services by property type
            _modelComposerResolver = modelComposerResolver;

            // Do the expensive reflection piece when this class is instantiated (as a singleton), ideally before serving requests
            _cachedPropertyInfos = typeof(TModel).GetProperties().Where(x => x.CanRead && x.CanWrite);
        }

        public override async Task<(TModel Model, int StatusCode)> HandleOfT(HttpContext context)
        {
            var result = new TModel();

            var pending = new List<Task>();

            foreach (var cachedPropertyInfo in _cachedPropertyInfos)
            {
                // Find a IComposeModel that can service this particular property type
                if (!_modelComposerResolver.TryResolve(cachedPropertyInfo.PropertyType, out var composer))
                {
                    //Log a warning 
                    continue;
                }

                pending.Add(AssignPropertyFromComposer(composer, cachedPropertyInfo, result, context.GetRouteData().Values));
            }

            if (pending.Count == 0)
            {
                return (null, StatusCodes.Status404NotFound);
            }

            await Task.WhenAll(pending);
            return (result, StatusCodes.Status200OK);
        }

        private static async Task AssignPropertyFromComposer(IComposeModel composer, PropertyInfo cachedPropertyInfo,
            TModel result, IDictionary<string, object> routeParameters)
        {
            var propertyValue = await composer.Compose(routeParameters);
            cachedPropertyInfo.SetValue(result, propertyValue);
        }
    }
}
