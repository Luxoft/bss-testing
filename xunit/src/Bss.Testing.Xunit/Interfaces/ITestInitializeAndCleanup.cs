namespace Bss.Testing.Xunit.Interfaces;

public interface ITestInitializeAndCleanup
{
    Task InitializeAsync();

    Task CleanupAsync();
}
