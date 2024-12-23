using Bss.Testing.Xunit.Interfaces;
using Bss.Testing.Xunit.Utils;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bss.Testing.Xunit.Runners;

public class TestMethodRunner(
    ITestMethod testMethod,
    IReflectionTypeInfo @class,
    IReflectionMethodInfo method,
    IEnumerable<IXunitTestCase> testCases,
    IMessageSink diagnosticMessageSink,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    object[] constructorArguments,
    IServiceProvider? testEnvServiceProvider)
    : TestMethodRunner<IXunitTestCase>(testMethod, @class, method, testCases, messageBus, aggregator, cancellationTokenSource)
{
    protected override async Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
    {
        if (testCase.GetType().GetInterface(nameof(ITheoryTestCase)) != null)
        {
            return await ((ITheoryTestCase)testCase).RunAsync(
                diagnosticMessageSink,
                this.MessageBus,
                constructorArguments,
                new ExceptionAggregator(this.Aggregator),
                this.CancellationTokenSource,
                testEnvServiceProvider);
        }

        return await testCase.RunAsync(
            diagnosticMessageSink,
            this.MessageBus,
            constructorArguments,
            new ExceptionAggregator(this.Aggregator),
            this.CancellationTokenSource);
    }

    protected override async void AfterTestMethodStarting()
    {
        try
        {
            await TestInitializationUtil.InitializeTest(testEnvServiceProvider, this.Aggregator);
            base.AfterTestMethodStarting();
        }
        catch (Exception e)
        {
            this.Aggregator.Add(e);
        }
    }

    protected override async void BeforeTestMethodFinished()
    {
        try
        {
            base.BeforeTestMethodFinished();
            await TestInitializationUtil.CleanupTest(testEnvServiceProvider, this.Aggregator);
        }
        catch (Exception e)
        {
            this.Aggregator.Add(e);
        }
    }
}
