using System.ComponentModel;

using Bss.Testing.Xunit.Interfaces;
using Bss.Testing.Xunit.Runners;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bss.Testing.Xunit.Sdk;

public class TheoryTestCase : XunitTheoryTestCase, ITheoryTestCase
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public TheoryTestCase() {}

    public TheoryTestCase(
        IMessageSink diagnosticMessageSink,
        TestMethodDisplay defaultMethodDisplay,
        TestMethodDisplayOptions defaultMethodDisplayOptions,
        ITestMethod testMethod)
        : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
    {
    }

    public async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
                                              IMessageBus messageBus,
                                              object[] constructorArguments,
                                              ExceptionAggregator aggregator,
                                              CancellationTokenSource cancellationTokenSource,
                                              IServiceProvider? testEnvServiceProvider)
        => await new TheoryTestCaseRunner(
            this,
            this.DisplayName,
            this.SkipReason,
            constructorArguments,
            diagnosticMessageSink,
            messageBus,
            aggregator,
            cancellationTokenSource,
            testEnvServiceProvider).RunAsync();
}
