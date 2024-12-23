namespace Bss.Testing.Xunit.Interfaces;

public interface ITestServiceProviderPool
{
    IServiceProvider? Get();
    void Release(IServiceProvider? serviceProvider);
}
