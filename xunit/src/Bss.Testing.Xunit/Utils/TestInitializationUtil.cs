using Bss.Testing.Xunit.Interfaces;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Sdk;

namespace Bss.Testing.Xunit.Utils;

public static class TestInitializationUtil
{
    public static async Task InitializeTest(IServiceProvider? testEnvServiceProvider, ExceptionAggregator aggregator)
    {
        if (testEnvServiceProvider?.GetService<ITestInitializeAndCleanup>() is {} initialization)
        {
            await aggregator.RunAsync(initialization.InitializeAsync);
        }
    }

    public static async Task CleanupTest(IServiceProvider? testEnvServiceProvider, ExceptionAggregator aggregator)
    {
        if (testEnvServiceProvider?.GetService<ITestInitializeAndCleanup>() is {} initialization)
        {
            await aggregator.RunAsync(initialization.CleanupAsync);
        }
    }
}
