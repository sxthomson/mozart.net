using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Mozart.Composition.AspNetCore.Mvc.Results.Abstractions;
using Mozart.Composition.Core.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.Filters
{
    ///  <inheritdoc cref="Attribute" />
    ///  <summary>
    ///  Tell Mozart to intercept the result for this action and auto compose based on the declared type.
    ///  Requires resolved services so you should make use of <see cref="TypeFilterAttribute"/> or <seealso cref="ServiceFilterAttribute"/>.
    ///  </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MozartComposeModelAttribute : Attribute, IFilterFactory
    {        
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException($"{nameof(serviceProvider)}");
            }
            var cachedServiceResolver = serviceProvider.GetService<ICachedServiceResolver<string, IHandleResult>>();
            if (cachedServiceResolver == null)
            {
                throw new ArgumentNullException(
                    $"{nameof(ICachedServiceResolver<string, IHandleResult>)} not present in service provider.");
            }
            return new CompositionResultActionFilter(cachedServiceResolver);
        }

        public bool IsReusable => true; 
    }
}
