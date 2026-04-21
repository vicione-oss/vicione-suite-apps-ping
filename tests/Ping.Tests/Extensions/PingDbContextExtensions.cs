using AutoFixture;
using Ping.Backend.DbContext;
using Ping.Public.Contracts;

namespace Ping.Tests.Extensions;

internal static class PingDbContextExtensions
{
    /// <summary>
    /// Seeds Pings
    /// </summary>
    /// <returns>
    /// The Id of the First Ping
    /// </returns>
    public static PingResult SeedPing(this IPingDbContext context, int seedPingsCount = 1)
    {
        if (context.Pings.Any())
        {
            return context.Pings.First();
        }

        var fixture = new Fixture();

        PingResult? firstPing = null;
        for (var i = 0; i < seedPingsCount; i++)
        {
            var ping = fixture.Create<PingResult>();
            context.Pings.Add(ping);
            if (i == 0)
                firstPing = ping;
        }

        context.SaveChanges();
        return firstPing ?? throw new InvalidOperationException("No ping was seeded");
    }
}
