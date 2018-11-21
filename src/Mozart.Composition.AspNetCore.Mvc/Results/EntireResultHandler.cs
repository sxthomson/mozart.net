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
        private readonly IServiceResolver<Type, IComposeViewModel> _viewModelCompositionServiceResolver;

        public EntireResultHandler(IServiceResolver<Type, IComposeViewModel> viewModelCompositionServiceResolver)
        {
            // The custom resolver that will get our view model composition services by property type
            _viewModelCompositionServiceResolver = viewModelCompositionServiceResolver;
        }

        public override async Task<(T ViewModel, int StatusCode)> HandleOfT(HttpContext context)
        {
            if (!_viewModelCompositionServiceResolver.TryResolve(typeof(T), out var entireViewModelHandler))
            {
                return (null, StatusCodes.Status404NotFound);
            }

            var result = (T) await entireViewModelHandler.ComposeViewModel(context.GetRouteData().Values);
            return (result, StatusCodes.Status200OK);
        }
    }
}
