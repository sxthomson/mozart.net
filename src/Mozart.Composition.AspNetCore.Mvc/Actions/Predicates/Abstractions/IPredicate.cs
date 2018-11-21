using System;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.Predicates.Abstractions
{
    public interface IPredicate<in T>
    {
        Func<T, bool> Predicate { get; }
    }
}
