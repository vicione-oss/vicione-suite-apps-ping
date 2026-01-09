using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ping.Backend.Consumers;
using Ping.Public.Requests;
using Sdk.Instance;
using Sdk.Testing.Backend;
using Xunit;

namespace Ping.Tests.Backend.Consumers;

public class GetSystemInformationConsumerTests
{
    private readonly Action<IBusRegistrationConfigurator> _configureServices;

    public GetSystemInformationConsumerTests()
    {
        var instanceProviderMock = Substitute.For<IInstanceInformationProvider>();
        instanceProviderMock.SetupGetInstanceInformation(InstanceType.Slave);

        _configureServices = cfg =>
        {
            cfg.AddConsumer<GetSystemInformationConsumer>();
            cfg.AddSingleton(instanceProviderMock);
        };
    }

    [Fact(Skip = "Extension to test instance dependent requests will be added with sdk > 0.25.0")]
    public async Task Command_should_be_consumed()
    {
        // Arrange
        await using var tester = new MassTransitTester(_configureServices);
        var request = new GetSystemInformationRequest(InstanceType.Master);

        // Act + Assert
        //var response = await tester
        //    .TestRequest<GetSystemInformationResponse, GetSystemInformationRequest>(request);

        //Assert.True(response.SysInfo.Contains("Pong requested", StringComparison.Ordinal));
    }
}
