using System;

namespace Mozart.Composition.Core.Exceptions
{
    public class RegisterServicesMethodTypeLoadException : TypeLoadException
    {
        public RegisterServicesMethodTypeLoadException()
        {
        }

        public RegisterServicesMethodTypeLoadException(string message) : base(message)
        {
        }
    }
}
