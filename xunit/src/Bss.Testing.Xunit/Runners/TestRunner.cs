using System.Reflection;

using Bss.Testing.Xunit.Utils;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bss.Testing.Xunit.Runners;

public class TestRunner(
    ITest test,
    IMessageBus messageBus,
    Type testClass,
    object[] constructorArguments,
    MethodInfo testMethod,
    object[] testMethodArguments,
    string skipReason,
    IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    IServiceProvider? testEnvServiceProvider)
    : XunitTestRunner(
        test,
        messageBus,
        testClass,
        constructorArguments,
        testMethod,
        testMethodArguments,
        skipReason,
        beforeAfterAttributes,
        aggregator,
        cancellationTokenSource)
{
    protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
    {
        await TestInitializationUtil.InitializeTest(testEnvServiceProvider, this.Aggregator);

        var result = await base.InvokeTestAsync(aggregator);

        await TestInitializationUtil.CleanupTest(testEnvServiceProvider, this.Aggregator);

        return result;
    }
}
