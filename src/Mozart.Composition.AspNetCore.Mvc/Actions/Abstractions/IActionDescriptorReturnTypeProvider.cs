using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.Abstractions
{
    public interface IActionDescriptorReturnTypeProvider<out T> where T : ActionDescriptor
    {
        IEnumerable<(string Id, Type ReturnType)> ResolveAll(Func<T, bool> predicate);
    }
}