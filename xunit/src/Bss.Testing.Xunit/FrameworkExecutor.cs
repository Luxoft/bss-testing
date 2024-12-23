using System.Reflection;

using Bss.Testing.Xunit.Interfaces;
using Bss.Testing.Xunit.Runners;
using Bss.Testing.Xunit.Utils;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bss.Testing.Xunit;

public class FrameworkExecutor : XunitTestFrameworkExecutor
{
    internal static IServiceProvider? FwServiceProvider { get; private set; }

    protected ExceptionAggregator Aggregator { get; set; } = new ExceptionAggregator();

    public FrameworkExecutor(
        AssemblyName assemblyName,
        ISourceInformationProvider sourceInformationProvider,
        IMessageSink diagnosticMessageSink)
        : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
    {
        try
        {
            FwServiceProvider = ReflectionUtils.GetImplementationInstanceOf<IFrameworkInitializer>(assemblyName)
                                               ?.GetFrameworkServiceProvider();
        }
        catch (Exception e)
        {
            this.DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Service Provider Initialization error: {e.Message}"));
            this.Aggregator.Add(e);
        }
    }

    protected override async void RunTestCases(
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions)
    {
        using var assemblyRunner = new TestAssemblyRunner(
            FwServiceProvider,
            this.TestAssembly,
            testCases,
            this.DiagnosticMessageSink,
            executionMessageSink,
            executionOptions,
            this.Aggregator);

        await assemblyRunner.RunAsync();
    }
}
