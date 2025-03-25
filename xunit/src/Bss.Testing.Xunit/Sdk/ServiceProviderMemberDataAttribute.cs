using System.Collections;
using System.Globalization;

using Xunit.Sdk;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Bss.Testing.Xunit.Sdk;

[DataDiscoverer("Bss.Testing.Xunit.Sdk.ServiceProviderMemberDataDiscoverer", "Bss.Testing.Xunit")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ServiceProviderMemberDataAttribute(string methodOrPropertyName) : DataAttribute
{
    public Type MemberType { get; set; }

    string MemberName { get; set; } = methodOrPropertyName;

    public override IEnumerable<object[]> GetData(MethodInfo testMethod) => null;

    public IEnumerable<object[]> GetData(MethodInfo testMethod, IServiceProvider serviceProvider)
    {
        var type = this.MemberType ?? testMethod.DeclaringType;
        var accessor = this.GetMethodAccessor(type, serviceProvider)
                       ?? this.GetPropertyAccessor(type, serviceProvider);

        if (accessor == null)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Could not find parameterless method or property named '{0}' on {1} provided in ServiceProviderMemberDataAttribute",
                    this.MemberName,
                    type?.FullName)
                );
        }

        var obj = accessor();
        if (obj == null)
        {
            return null;
        }

        if (obj is not IEnumerable dataItems)
        {
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Method/property {0} on {1} did not return IEnumerable", this.MemberName, type?.FullName));
        }

        return dataItems.Cast<object>().Select(item => this.ConvertDataItem(testMethod, item));
    }

    private Func<object?>? GetMethodAccessor(Type type, IServiceProvider serviceProvider)
    {
        MethodInfo? methodInfo = null;
        for (var reflectionType = type; reflectionType != null; reflectionType = reflectionType.GetTypeInfo().BaseType)
        {
            methodInfo = reflectionType
                             .GetRuntimeMethods()
                             .FirstOrDefault(m => m.Name == this.MemberName);
            if (methodInfo != null)
            {
                break;
            }
        }

        if (methodInfo == null)
        {
            return null;
        }

        var @object = ActivatorUtilities.CreateInstance(serviceProvider, type);

        return () => methodInfo.Invoke(@object, null);
    }

    private Func<object?>? GetPropertyAccessor(Type type, IServiceProvider serviceProvider)
    {
        PropertyInfo? propertyInfo = null;
        for (var reflectionType = type; reflectionType != null; reflectionType = reflectionType.GetTypeInfo().BaseType)
        {
            propertyInfo = reflectionType
                         .GetProperties()
                         .FirstOrDefault(m => m.Name == this.MemberName);

            if (propertyInfo != null)
            {
                break;
            }
        }

        if (propertyInfo == null)
        {
            return null;
        }

        var @object = ActivatorUtilities.CreateInstance(serviceProvider, type);

        return () => propertyInfo.GetValue(@object);
    }

    private object[]? ConvertDataItem(MethodInfo testMethod, object? item)
    {
        if (item == null)
        {
            return null;
        }

        if (item is not object[] array)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "Method {0} on {1} yielded an item that is not an object[]",
                    this.MemberName,
                    this.MemberType ?? testMethod.DeclaringType
                    ));
        }

        return array;
    }
}
