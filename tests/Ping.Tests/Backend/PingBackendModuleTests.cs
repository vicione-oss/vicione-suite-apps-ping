using Microsoft.Extensions.DependencyInjection;
using Ping.Backend;
using Ping.Backend.DbContext;
using Sdk.Testing.Backend;
using Xunit;

namespace Ping.Tests.Backend;

public class PingBackendModuleTests
{
    [Fact]
    public void Init_module_should_register_and_configure_services()
    {
        // Arrange
        var module = new PingBackendModule();

        // Act
        var serviceProvider = module.TestModuleInitialization();

        // Assert
        Assert.NotNull(module.ModuleKey.ModuleId);
        Assert.NotNull(module.ModuleInitializer);

        Assert.NotNull(serviceProvider.GetRequiredService<IPingDbContext>());
    }
}
