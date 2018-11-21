using System;
using System.Collections.Generic;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies.Abstractions
{
    public interface IUnwrapReturnType
    {
        Type GetUnwrappedReturnType(Type returnType, IEnumerable<Attribute> attributes);
    }
}