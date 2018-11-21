using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mozart.Composition.AspNetCore.Mvc.Results.Abstractions;
using Mozart.Composition.Core.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder WarmupMozartModelComposition(this IApplicationBuilder builder)
        {
            builder.ApplicationServices.GetService<ICachedServiceResolver<string, IHandleResult>>();
            builder.ApplicationServices.GetService<ICachedServiceResolver<Type, IComposeModel>>();
            return builder;
        }
    }
}
