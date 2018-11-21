using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mozart.Composition.AspNetCore.Mvc.UnitTests.Fakes
{
    // Simplify mocking MethodInfo and Attributes by providing known fakes to use
    // when tests are invoked.
    public class FakeController : Controller
    {
        public Task<string> ActionWithNoAttributesReturnTaskString()
        {
            return null;
        }

        [HttpGet]
        public Task<string> ActionWithHttpGetAttributeReturnTaskString()
        {
            return null;
        }

        public Task<DateTime> ActionWithNoAttributesReturnTaskDateTime()
        {
            return null;
        }

        [HttpGet]
        public Task<DateTime> ActionWithHttpGetAttributeReturnTaskDateTime()
        {
            return null;
        }
    }
}
