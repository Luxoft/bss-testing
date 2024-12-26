# Integration Tests Xunit
 - Implement IFrameworkInitializer somewhere in your project.
 - Add [assembly: TestFramework("Bss.Testing.Xunit.TestFramework", "Bss.Testing.Xunit")]
 - xUnit [Theory] should be replaced by [Bss.Testing.Xunit.Sdk.Theory] in order to use ability to initialize tests via injected ITestInitializeAndCleanup instead of Fixture/TestBase implementation
Example:
```
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerClass)]
[assembly: TestFramework("Bss.Testing.Xunit.TestFramework", "Bss.Testing.Xunit")]

public class EnvironmentInitializer : IFrameworkInitializer
{
    public IServiceCollection ConfigureFramework(IServiceCollection services) =>
        services
            .AddSingleton<IAssemblyInitializeAndCleanup, DiAssemblyInitializeAndCleanup>()
            .AddSingleton<ITestDatabaseGenerator, DatabaseGenerator>()
            .AddSingleton<IConfiguration>(
                new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false)
                    .AddEnvironmentVariables("LS_")
                    .Build());

    public IServiceProvider ConfigureTestEnvironment(IServiceCollection services, IConfiguration configuration) =>
        services
            .AddApplication(configuration, new HostingEnvironment { EnvironmentName = Environments.Development })
            .AddIntegrationTestServices(
                options =>
                {
                    options.IntegrationTestUserName = TestConstants.Principals.Admin.Name;
                })
            .BuildServiceProvider();
}
```