using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Ping.Backend.Consumers;
using Ping.Backend.DbContext;
using Ping.Public.Commands;
using Ping.Public.Contracts;
using Ping.Public.Events;
using Sdk.Backend.Messaging;
using Sdk.Instance;
using Sdk.Testing.Backend;
using Xunit;

namespace Ping.Tests.Backend.Consumers;

public class CreatePingConsumerTests : TestWithDbContextSqlite<PingDbContextSqlite>
{
    private Action<IBusRegistrationConfigurator> _configureServices;

    public CreatePingConsumerTests()
    {
        var instanceProviderMock = Substitute.For<IInstanceInformationProvider>();

        instanceProviderMock.GetInstancesInCluster(Arg.Any<CancellationToken>())
            .Returns([]);

        _configureServices = cfg =>
        {
            cfg.AddConsumer<CreatePingConsumer>();
            cfg.AddSingleton(instanceProviderMock);
            cfg.AddSingleton(Substitute.For<ISuiteMediator>());
            cfg.AddSingleton<IPingDbContext>(_ => TestDbContext);
        };
    }

    [Fact]
    public async Task Command_should_be_consumed()
    {
        // Arrange
        await using var tester = new MassTransitTester(_configureServices);
        var dbContext = tester.Services.GetRequiredService<IPingDbContext>();
        var command = new CreatePing(Guid.NewGuid(), 100);

        // Act + Assert
        await tester.TestCommand<CreatePing, CreatePingConsumer>(command, command.PingId);

        Assert.False(await tester.Harness.Published.Any<PingError>(TestContext.Current.CancellationToken));
        Assert.True(await tester.Harness.Published.Any<PingCreated>(TestContext.Current.CancellationToken));
        Assert.True(await dbContext.Pings.AnyAsync(p => p.Id == command.PingId, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Consume_should_log_warning_on_failure()
    {
        // Arrange
        var pingId = Guid.NewGuid();

        var dbContextMock = Substitute.For<IPingDbContext>();
        dbContextMock.Pings.Add(Arg.Do<PingResult>(_ => throw new DbUpdateException("test")));

        var loggerMock = Substitute.For<ILogger<CreatePingConsumer>>();

        _configureServices += cfg =>
        {
            cfg.AddSingleton(dbContextMock);
            cfg.AddSingleton(loggerMock);
        };

        await using var tester = new MassTransitTester(_configureServices);

        var command = new CreatePing(pingId, 100);

        // Act
        await Assert.ThrowsAsync<DbUpdateException>(() => tester.TestCommandFault<CreatePing, CreatePingConsumer>(command, command.PingId));
    }

    [Fact]
    public async Task Fault_consumer_should_publish_ping_error()
    {
        var pingId = Guid.NewGuid();

        var dbContextMock = Substitute.For<IPingDbContext>();
        dbContextMock.Pings.Add(Arg.Do<PingResult>(_ => throw new DbUpdateException("test")));

        _configureServices += cfg => cfg.AddSingleton(dbContextMock);

        await using var tester = new MassTransitTester(_configureServices);

        var command = new CreatePing(pingId, 100);

        // Act
        await Assert.ThrowsAsync<DbUpdateException>(() => tester.TestCommandFault<CreatePing, CreatePingConsumer>(command, command.PingId));

        // Assert
        Assert.True(await tester.Harness.Published.Any<PingError>(TestContext.Current.CancellationToken));
    }
}
