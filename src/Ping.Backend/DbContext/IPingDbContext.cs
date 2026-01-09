using Microsoft.EntityFrameworkCore;
using Ping.Public.Contracts;
using Sdk.Backend.Persistence;

namespace Ping.Backend.DbContext;

//Mock this for unit tests:
public interface IPingDbContext : IModuleDbContext
{
    public DbSet<PingResult> Pings { get; }
}