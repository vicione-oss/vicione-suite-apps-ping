using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Ping.Backend.Consumers;
using Ping.Backend.DbContext;
using Ping.Public.Commands;
using Ping.Public.Events;
using Ping.Tests.Extensions;
using Sdk.Testing.Backend;
using Xunit;

namespace Ping.Tests.Backend.Consumers;

public sealed class RemovePingConsumerTests : TestWithDbContextSqlite<PingDbContextSqlite>
{
    private readonly Action<IBusRegistrationConfigurator> _configureServices;
    public RemovePingConsumerTests()
    {
        _configureServices = cfg =>
        {
            // consumer needs
            cfg.AddConsumer<RemovePingConsumer>();
            cfg.AddSingleton<IPingDbContext>(_ => TestDbContext);
        };
    }

    [Fact]
    public async Task Command_should_be_consumed()
    {
        // Arrange
        await using var tester = new MassTransitTester(_configureServices);
        var command = new RemovePing(Guid.NewGuid());

        // Act
        await tester.TestCommand<RemovePing, RemovePingConsumer>(command, command.PingId);

        // Assert
        var consumerHarness = tester.Services.GetRequiredService<IConsumerTestHarness<RemovePingConsumer>>();

        // did the actual consumer consume the message
        Assert.True(await consumerHarness.Consumed.Any<RemovePing>(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Delete_existing_ping_should_publish_deleted_event()
    {
        // Arrange
        await using var tester = new MassTransitTester(_configureServices);
        var dbContext = tester.Services.GetRequiredService<IPingDbContext>();
        var ping = dbContext.SeedPing();
        var command = new RemovePing(ping.Id);

        // Act
        await tester.TestCommand<RemovePing, RemovePingConsumer>(command, command.PingId);

        // Assert
        Assert.False(await tester.Harness.Published.Any<Fault<PingError>>(TestContext.Current.CancellationToken));
        Assert.True(await tester.Harness.Published.Any<PingDeleted>(r =>
            Equals(r.Context.Message.PingId, ping.Id), TestContext.Current.CancellationToken));
        Assert.False(await dbContext.Pings.AnyAsync(p => p.Id == ping.Id, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Delete_unknown_ping_should_not_publish_deleted_event()
    {
        // Arrange
        await using var tester = new MassTransitTester(_configureServices);
        var command = new RemovePing(Guid.NewGuid());

        // Act
        await tester.TestCommand<RemovePing, RemovePingConsumer>(command, command.PingId);

        // Assert
        Assert.False(await tester.Harness.Published.Any<Fault<PingError>>(TestContext.Current.CancellationToken));
        Assert.False(await tester.Harness.Published.Any<PingDeleted>(TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// maybe it will disappear in v8 - not sure
    /// </summary>
    [Fact]
    public async Task Test_consumer_with_memory_test_harness()
    {
        // Arrange
        using var harness = new InMemoryTestHarness();
        var loggerMock = Substitute.For<ILogger<RemovePingConsumer>>();

        // add the consumer services
        await using var serviceProvider = new ServiceCollection()
            .AddSingleton(loggerMock)
            .AddSingleton<IPingDbContext>(_ => TestDbContext)
            .AddSingleton<RemovePingConsumer>()
            .BuildServiceProvider();

        // create test harness for the consumer
        var consumer = serviceProvider.GetRequiredService<RemovePingConsumer>();
        var consumerHarness = harness.Consumer(() => consumer);

        await harness.Start(TestContext.Current.CancellationToken);
        try
        {
            // put the command to the test harness consumer bus
            await harness.InputQueueSendEndpoint.Send(new RemovePing(Guid.NewGuid()), TestContext.Current.CancellationToken);

            // check if the command was consumed
            Assert.True(await consumerHarness.Consumed.Any<RemovePing>(TestContext.Current.CancellationToken));
        }
        finally
        {
            // InMemoryTestHarness needs to be shut down
            await harness.Stop();
        }
    }
}
