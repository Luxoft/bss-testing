using System.ComponentModel;

using Bss.Testing.Xunit.Interfaces;
using Bss.Testing.Xunit.Runners;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bss.Testing.Xunit.Sdk;

public class TestCase : XunitTestCase, ITheoryTestCase
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public TestCase()
    {
    }

    public TestCase(
        IMessageSink diagnosticMessageSink,
        TestMethodDisplay defaultMethodDisplay,
        TestMethodDisplayOptions defaultMethodDisplayOptions,
        ITestMethod testMethod,
        object[]? testMethodArguments = null)
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
    {
    }

    public async Task<RunSummary> RunAsync(
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        object[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IServiceProvider? testEnvServiceProvider)
        => await new TestCaseRunner(
            this,
            this.DisplayName,
            this.SkipReason,
            constructorArguments,
            this.TestMethodArguments,
            messageBus,
            aggregator,
            cancellationTokenSource,
            testEnvServiceProvider).RunAsync();
}
