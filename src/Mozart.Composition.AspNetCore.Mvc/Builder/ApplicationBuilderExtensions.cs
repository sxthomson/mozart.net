using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mozart.Composition.AspNetCore.Mvc.Results.Abstractions;
using Mozart.Composition.Core.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder WarmupViewModelComposition(this IApplicationBuilder builder)
        {
            builder.ApplicationServices.GetService<IServiceResolver<string, IHandleResult>>();
            builder.ApplicationServices.GetService<IServiceResolver<Type, IComposeViewModel>>();
            return builder;
        }
    }
}
