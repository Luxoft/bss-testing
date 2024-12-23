using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bss.Testing.Xunit.Interfaces;

public interface ITheoryTestCase : IXunitTestCase
{
    Task<RunSummary> RunAsync(
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        object[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IServiceProvider? testEnvServiceProvider);
}
