using System;

namespace Mozart.Composition.AspNetCore.Mvc.Exceptions
{
    public class MissingExpectedAttributeException : InvalidOperationException
    {
        public MissingExpectedAttributeException(string message) : base(message)
        {
        }
    }
}
