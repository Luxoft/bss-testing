﻿using System.Reflection;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bss.Testing.Xunit.Runners;

public class TestCaseRunner(
    IXunitTestCase testCase,
    string displayName,
    string skipReason,
    object[] constructorArguments,
    object[] testMethodArguments,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    IServiceProvider? testEnvServiceProvider)
    : XunitTestCaseRunner(
        testCase,
        displayName,
        skipReason,
        constructorArguments,
        testMethodArguments,
        messageBus,
        aggregator,
        cancellationTokenSource)
{
    protected readonly IServiceProvider? TestEnvServiceProvider = testEnvServiceProvider;

    protected override XunitTestRunner CreateTestRunner(
        ITest test,
        IMessageBus messageBus,
        Type testClass,
        object[] constructorArguments,
        MethodInfo testMethod,
        object[] testMethodArguments,
        string skipReason,
        IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
        => new TestRunner(
            test,
            messageBus,
            testClass,
            constructorArguments,
            testMethod,
            testMethodArguments,
            skipReason,
            beforeAfterAttributes,
            new ExceptionAggregator(aggregator),
            cancellationTokenSource,
            this.TestEnvServiceProvider);
}
