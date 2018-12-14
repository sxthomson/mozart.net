using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using Mozart.Composition.AspNetCore.Mvc.Actions.Predicates.Abstractions;

namespace Mozart.Composition.AspNetCore.Mvc.Actions.Predicates
{
    public class ControllerActionDescriptorHttpGetPredicateWrapper : IPredicate<ControllerActionDescriptor>
    {
        public Func<ControllerActionDescriptor, bool> Predicate => x =>
        {
            if (x.ActionConstraints == null || x.ActionConstraints.Any(ac =>
                    ac.GetType() == typeof(HttpMethodActionConstraint)))
            {
                return true; // Assume this route can be accessed via GET
            }

            return x.ActionConstraints.Any(ac =>
                ac.GetType() == typeof(HttpMethodActionConstraint) &&
                ((HttpMethodActionConstraint)ac).HttpMethods.Any(HttpMethods.IsGet));
        };
    }
}
