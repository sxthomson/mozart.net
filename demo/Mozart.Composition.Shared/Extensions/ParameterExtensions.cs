using System;
using System.Collections.Generic;

namespace Mozart.Composition.Shared.Extensions
{
    public static class ParameterExtensions
    {
        public static int GetId(this IDictionary<string, object> parameters)
        {
            return int.Parse(parameters["id"].ToString());
        }

        public static T GetParameterOrThrow<T>(this IDictionary<string, object> parameters, string parameterName)
        {
            var objectParameter = parameters[parameterName];

            if (objectParameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return (T) objectParameter;
        }
    }
}
