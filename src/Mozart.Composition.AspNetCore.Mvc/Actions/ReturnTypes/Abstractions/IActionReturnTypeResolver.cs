using System;
using System.Collections.Generic;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Abstractions
{
    public interface IActionReturnTypeResolver
    {
        Type ResolveUnwrappedReturnType(Type returnType, IEnumerable<Attribute> attributes);
    }
}