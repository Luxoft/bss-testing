namespace Bss.Testing.Xunit.Interfaces;

public interface IAssemblyInitializeAndCleanup
{
    Task EnvironmentInitializeAsync();

    Task EnvironmentCleanupAsync();
}
