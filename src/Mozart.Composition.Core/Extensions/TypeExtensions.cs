using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mozart.Composition.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            if (interfaceType.IsAssignableFrom(type)) return true;

            return type
                .GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
        }

        public static bool IsConcreteAndAssignableFrom(this Type type, Type implementedType)
        {
            var typeInfo = type.GetTypeInfo();
            return !typeInfo.IsInterface
                   && !typeInfo.IsAbstract
                   && (implementedType.IsAssignableFrom(type) ||
                       IsAssignableToGenericType(type, implementedType));
        }

        public static IEnumerable<Type> GetImplementedGenericArgumentsForInterface(this Type type, Type interfaceType)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException("Supplied argument is not an interface type");
            }

            if (!interfaceType.ContainsGenericParameters)
            {
                throw new ArgumentException("Supplied interface argument does not contain generic parameters");
            }

            return type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
                .SelectMany(i => i.GetGenericArguments());
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
            {
                return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }

        public static bool HasAttribute<TAttribute>(this Type memberType) where TAttribute : Attribute
        {
            return Attribute.GetCustomAttribute(memberType, typeof(TAttribute)) != null;
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo memberInfo) where TAttribute : Attribute
        {
            return HasAttribute<TAttribute>(memberInfo.GetType());
        }
    }
}
