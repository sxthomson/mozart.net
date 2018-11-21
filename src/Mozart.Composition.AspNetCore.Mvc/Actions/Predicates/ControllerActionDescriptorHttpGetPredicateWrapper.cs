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
            x.ActionConstraints.Any(ac =>
                ac.GetType() == typeof(HttpMethodActionConstraint) &&
                ((HttpMethodActionConstraint) ac).HttpMethods.Any(HttpMethods.IsGet));
    }
}
