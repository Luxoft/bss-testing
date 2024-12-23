using Bss.Testing.Xunit.Interfaces;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bss.Testing.Xunit.Sdk;

public class TheoryDiscoverer : global::Xunit.Sdk.TheoryDiscoverer
{
    public TheoryDiscoverer(IMessageSink diagnosticMessageSink)
        : base(diagnosticMessageSink)
    {
    }

    protected override IEnumerable<ITheoryTestCase> CreateTestCasesForTheory(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod testMethod,
        IAttributeInfo theoryAttribute)
        => new[]
           {
               new TheoryTestCase(
                   this.DiagnosticMessageSink,
                   discoveryOptions.MethodDisplayOrDefault(),
                   discoveryOptions.MethodDisplayOptionsOrDefault(),
                   testMethod)
           };

    protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod testMethod,
        IAttributeInfo theoryAttribute,
        object[]? dataRow)
        => new[]
           {
               new TestCase(
                   this.DiagnosticMessageSink,
                   discoveryOptions.MethodDisplayOrDefault(),
                   discoveryOptions.MethodDisplayOptionsOrDefault(),
                   testMethod,
                   dataRow)
           };
}

