using AwesomeAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ping.Backend.Consumers;
using Ping.Backend.DbContext;
using Ping.Public.Events;
using Ping.Public.Requests;
using Ping.Tests.Extensions;
using Sdk.Testing.Backend;
using Xunit;

namespace Ping.Tests.Backend.Consumers;

public class GetPingsConsumerTests : TestWithDbContextSqlite<PingDbContextSqlite>
{
    private Action<IBusRegistrationConfigurator> _configureServices;

    public GetPingsConsumerTests()
    {
        _configureServices = cfg =>
        {
            cfg.AddConsumer<GetPingsConsumer>();
            cfg.AddSingleton<IPingDbContext>(_ => TestDbContext);
        };
    }

    [Fact]
    public async Task Id_should_return_single_ping()
    {
        // Arrange
        await using var tester = new MassTransitTester(_configureServices);
        var ping = tester.Services.GetRequiredService<IPingDbContext>().SeedPing(4);
        var request = new GetPingsRequest(ping.Id);

        // Act
        var response = await tester.TestRequest<GetPingsResponse, GetPingsRequest>(request);

        // Assert
        Assert.Single(response.Result);
        response.Result[0].Should().BeEquivalentTo(ping);
    }

    [Fact]
    public async Task No_id_should_return_all_pings()
    {
        // Arrange
        await using var tester = new MassTransitTester(_configureServices);
        tester.Services.GetRequiredService<IPingDbContext>().SeedPing(4);
        var request = new GetPingsRequest(null);

        // Act
        var response = await tester.TestRequest<GetPingsResponse, GetPingsRequest>(request);

        // Assert
        response.Result
            .Should()
            .NotBeEmpty()
            .And.HaveCount(4);
    }

    [Fact]
    public async Task Should_publish_response_on_failure()
    {
        // Arrange
        var dbContextMock = Substitute.For<IPingDbContext>();

        dbContextMock.Pings
            .Returns(_ => throw new ArgumentException("test"));

        _configureServices += cfg => cfg.AddSingleton(dbContextMock);

        await using var tester = new MassTransitTester(_configureServices);
        var request = new GetPingsRequest(Guid.NewGuid());

        // Act
        var response = await tester.TestRequest<GetPingsResponse, GetPingsRequest>(request);

        // Assert
        response.Should().BeEquivalentTo(new GetPingsResponse([], new(PingError.UnknownError, "test")));
    }

    [Fact]
    public async Task Unknown_id_should_return_empty_list()
    {
        // Arrange
        await using var tester = new MassTransitTester(_configureServices);
        var request = new GetPingsRequest(Guid.NewGuid());

        // Act
        var response = await tester.TestRequest<GetPingsResponse, GetPingsRequest>(request);

        // Assert
        Assert.Empty(response.Result);
    }
}
