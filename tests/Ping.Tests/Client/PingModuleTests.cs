using Microsoft.Extensions.DependencyInjection;
using Ping.Client;
using Ping.Client.Ping;
using Sdk.Client.NavTiles.Services;
using Sdk.Testing.Client;
using Xunit;

namespace Ping.Tests.Client;

public class PingModuleTests
{
    [Fact]
    public void Init_module_should_register_and_configure_services()
    {
        // Arrange
        var module = new PingClientModule();

        // Act
        var serviceProvider = module.TestModuleInitialization(null);

        // Assert
        Assert.NotNull(serviceProvider.GetRequiredService<PingState>());
    }
}
