using System.Reflection;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Bss.Testing.Xunit;

public class TestFramework(IMessageSink messageSink) : XunitTestFramework(messageSink)
{
    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName) =>
        new FrameworkExecutor(assemblyName, this.SourceInformationProvider, this.DiagnosticMessageSink);
}
