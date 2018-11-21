using System;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.ReturnTypes.Strategies.Abstractions
{
    public interface IHandleType
    {
        bool Handles(Type type);
    }
}