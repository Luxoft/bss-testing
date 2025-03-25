using Xunit;
using Xunit.Sdk;

namespace Bss.Testing.Xunit.Sdk;

[XunitTestCaseDiscoverer("Bss.Testing.Xunit.Sdk.TheoryDiscoverer", "Bss.Testing.Xunit")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class BssTheoryAttribute : FactAttribute
{
}
