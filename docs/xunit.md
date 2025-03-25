# Integration Tests Xunit

This is a custom framework for [xUnit](https://github.com/xunit/xunit) that allows to inject IServiceProvider into test classes.

**How to use:**
 - Implement IFrameworkInitializer somewhere in your project.
 - Add [assembly: TestFramework("Bss.Testing.Xunit.TestFramework", "Bss.Testing.Xunit")] attribute.
 - xUnit [Theory] should be replaced by [Bss.Testing.Xunit.Sdk.Theory] in order to use ability to initialize tests via injected ITestInitializeAndCleanup instead of Fixture/TestBase implementation
 
**Initializer Example:**
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

**Parameterized tests with data from the IServiceProvider:**
Create a instance method or property that returns IEnumerable<object>. Pass that method to ServiceProviderMemberDataAttribute to use it as data source for Theory.

```
[Bss.Testing.Xunit.Sdk.Theory]
[ServiceProviderMemberData(nameof(GetMemberData))]
public void GetDataFromServiceProvider(FullSecurityRole role) => Assert.NotEmpty(role.Name);

protected IEnumerable<object> GetMemberData() =>
    this.ServiceProvider.GetRequiredService<ISecurityRoleSource>().SecurityRoles.Select(x => new [] { x });
```

```
[Bss.Testing.Xunit.Sdk.Theory]
[ServiceProviderMemberData(nameof(GetMemberData))]
public void GetDataFromServiceProvider(FullSecurityRole role) => Assert.NotEmpty(role.Name);

protected IEnumerable<object> GetMemberData => this.ServiceProvider.GetRequiredService<ISecurityRoleSource>().SecurityRoles.Select(x => new [] { x });
```