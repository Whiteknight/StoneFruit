using System;
using System.Linq;
using System.Reflection;

namespace StoneFruit.Utility
{
    // a poor-mans DI/IoC "container". Given a list of available values and a type whose constructors may
    // require one or more of those values, attempt to construct an instance of that type.
    public static class DuckTypeConstructorInvoker
    {
        public static object TryConstruct(Type typeToConstruct, object[] availableArguments)
        {
            var constructors = typeToConstruct.GetConstructors().OrderByDescending(ci => ci.GetParameters().Length).ToList();
            foreach (var constructor in constructors)
            {
                var result = TryConstruct(constructor, availableArguments);
                if (result != null)
                    return result;
            }

            return null;
        }

        private static object TryConstruct(ConstructorInfo constructor, object[] availableArguments)
        {
            var parameters = constructor.GetParameters();
            var arguments = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var value = TryGetValue(parameter.ParameterType, availableArguments);
                if (value == null)
                    return null;
                arguments[i] = value;
            }

            return constructor.Invoke(arguments);
        }

        private static object TryGetValue(Type parameterType, object[] availableArguments)
        {
            foreach (var arg in availableArguments)
            {
                if (arg == null)
                    continue;
                if (parameterType.IsInstanceOfType(arg))
                    return arg;
            }

            return null;
        }
    }
}