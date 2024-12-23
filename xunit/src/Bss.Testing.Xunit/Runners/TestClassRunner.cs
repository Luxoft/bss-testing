using System.Reflection;

using Bss.Testing.Xunit.Interfaces;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bss.Testing.Xunit.Runners;

public class TestClassRunner : XunitTestClassRunner
{
    private readonly ITestServiceProviderPool? serviceProviderPool;

    private readonly IServiceProvider? testEnvServiceProvider;

    public TestClassRunner(
        IServiceProvider? fwServiceProvider,
        ITestClass testClass,
        IReflectionTypeInfo @class,
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        ITestCaseOrderer testCaseOrderer,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IDictionary<Type, object> collectionFixtureMappings)
        : base(
            testClass,
            @class,
            testCases,
            diagnosticMessageSink,
            messageBus,
            testCaseOrderer,
            aggregator,
            cancellationTokenSource,
            collectionFixtureMappings)
    {
        if (fwServiceProvider is null)
        {
            return;
        }

        this.serviceProviderPool = fwServiceProvider.GetService<ITestServiceProviderPool>();

        try
        {
            if (this.serviceProviderPool is not null)
            {
                this.testEnvServiceProvider = this.serviceProviderPool.Get();
            }
        }
        catch (Exception e)
        {
            this.Aggregator.Add(e);
        }
    }

    protected override async Task<RunSummary> RunTestMethodAsync(
        ITestMethod testMethod,
        IReflectionMethodInfo method,
        IEnumerable<IXunitTestCase> testCases,
        object[] constructorArguments)
        => await new TestMethodRunner(
            testMethod,
            this.Class,
            method,
            testCases,
            this.DiagnosticMessageSink,
            this.MessageBus,
            new ExceptionAggregator(this.Aggregator),
            this.CancellationTokenSource,
            constructorArguments,
            this.testEnvServiceProvider)
            .RunAsync();

    protected override bool TryGetConstructorArgument(ConstructorInfo constructor, int index, ParameterInfo parameter, out object? argumentValue)
    {
        if (parameter.ParameterType == typeof(ITestOutputHelper))
        {
            argumentValue = new TestOutputHelper();
            return true;
        }

        if (parameter.ParameterType == typeof(IServiceProvider))
        {
            argumentValue = this.testEnvServiceProvider;
            return true;
        }

        return this.ClassFixtureMappings.TryGetValue(parameter.ParameterType, out argumentValue!)
               || this.TryGetValueFromServiceProvider(parameter.ParameterType, out argumentValue);
    }

    protected override async Task BeforeTestClassFinishedAsync()
    {
        await base.BeforeTestClassFinishedAsync();
        this.serviceProviderPool?.Release(this.testEnvServiceProvider);
    }

    private bool TryGetValueFromServiceProvider(Type type, out object? argumentValue)
    {
        if (this.testEnvServiceProvider?.GetService(type) is { } service)
        {
            argumentValue = service;
            return true;
        }

        argumentValue = null;
        return false;
    }
}
