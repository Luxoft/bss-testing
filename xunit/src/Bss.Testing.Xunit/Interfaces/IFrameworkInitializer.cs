using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bss.Testing.Xunit.Interfaces;

public interface IFrameworkInitializer
{
    public IServiceCollection ConfigureFramework(IServiceCollection services);

    public IServiceProvider ConfigureTestEnvironment(IServiceCollection services, IConfiguration configuration);

    public IServiceProvider GetFrameworkServiceProvider();
}
