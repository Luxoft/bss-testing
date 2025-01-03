﻿using System.Reflection;

using Bss.Testing.Xunit.Exceptions;

namespace Bss.Testing.Xunit.Utils;

public static class ReflectionUtils
{
    public static Type GetSingleTypeImplementation<T>(AssemblyName assemblyName)
    {
        var implementations = GetAllTypeImplementation<T>(assemblyName);

        if (implementations.Count > 1)
        {
            throw new BssTestingFrameworkException(
                $"Initialization error. Test project must contains single {nameof(T)} implementation");
        }

        return implementations.First();
    }

    public static T? GetImplementationInstanceOf<T>(AssemblyName assemblyName) where T : class
    {
        var types = GetAllTypeImplementation<T>(assemblyName);

        if (types.Count != 1)
        {
            throw new BssTestingFrameworkException(
                $"Initialization error. Test project must contains single {nameof(T)} implementation");
        }

        return GetObjectInstance<T>(types.First());
    }

    private static List<Type> GetAllTypeImplementation<T>(AssemblyName assemblyName)
    {
        var assembly = Assembly.Load(assemblyName);
        return assembly.GetTypes()
                                      .Where(type => typeof(T).IsAssignableFrom(type))
                                      .ToList();
    }

    public static T? GetObjectInstance<T>(Type initializationType)
    {
        var constructors = initializationType.GetConstructors();
        if (constructors.Length != 1 || constructors.First().GetParameters().Length != 0)
        {
            throw new BssTestingFrameworkException(
                $"Initialization error. {initializationType.FullName} must contain single public parameterless constructor.");
        }

        return (T?)Activator.CreateInstance(initializationType);
    }

    public static MethodInfo GetSingleMethod(Type type, string methodName)
    {
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                          .Where(m => m.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase)).ToList();

        if (methods.Count != 1)
        {
            throw new BssTestingFrameworkException(
                $"Initialization error. Implementation of {type.FullName} must contains single {methodName} method.");
        }

        return methods.First();
    }
}
