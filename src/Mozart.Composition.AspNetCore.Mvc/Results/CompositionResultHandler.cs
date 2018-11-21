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
    public class CompositionResultHandler<TViewModel> : CompositionResultHandlerBase<TViewModel> where TViewModel : class, new()
    {
        private readonly IServiceResolver<Type, IComposeViewModel> _viewModelComposerResolver;
        private readonly IEnumerable<PropertyInfo> _cachedPropertyInfos;

        public CompositionResultHandler(IServiceResolver<Type, IComposeViewModel> viewModelComposerResolver)
        {
            // The custom resolver that will get our view model composition services by property type
            _viewModelComposerResolver = viewModelComposerResolver;

            // Do the expensive reflection piece when this class is instantiated (as a singleton), ideally before serving requests
            _cachedPropertyInfos = typeof(TViewModel).GetProperties().Where(x => x.CanRead && x.CanWrite);
        }

        public override async Task<(TViewModel ViewModel, int StatusCode)> HandleOfT(HttpContext context)
        {
            var result = new TViewModel();

            var pending = new List<Task>();

            foreach (var cachedPropertyInfo in _cachedPropertyInfos)
            {
                // Find a IComposeViewModel that can service this particular property type
                if (!_viewModelComposerResolver.TryResolve(cachedPropertyInfo.PropertyType, out var composer))
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

        private static async Task AssignPropertyFromComposer(IComposeViewModel composer, PropertyInfo cachedPropertyInfo,
            TViewModel result, IDictionary<string, object> routeParameters)
        {
            var propertyValue = await composer.ComposeViewModel(routeParameters);
            cachedPropertyInfo.SetValue(result, propertyValue);
        }
    }
}
