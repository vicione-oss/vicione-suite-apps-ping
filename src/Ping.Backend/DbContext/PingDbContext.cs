using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Ping.Public.Contracts;
using Sdk.Backend.Persistence;

namespace Ping.Backend.DbContext;

// To add migrations see Readme.md section Module-Migration
public class PingDbContext : ModuleDbContext, IPingDbContext
{
    public override string DefaultSchemaName => "ping";
    public DbSet<PingResult> Pings => Set<PingResult>();

    internal PingDbContext(DbContextOptions options)
        : base(options)
    {
    }
}

public sealed class PingDbContextSqlite(DbContextOptions<PingDbContextSqlite> options) : PingDbContext(options), ISqliteDbContext
{
}

public sealed class PingDbContextPostgres(DbContextOptions<PingDbContextPostgres> options) : PingDbContext(options), IPostgresDbContext
{
}

public sealed class PingDbContextPostgresFactory : IDesignTimeDbContextFactory<PingDbContextPostgres>
{
    public PingDbContextPostgres CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PingDbContextPostgres>();
        optionsBuilder.UseNpgsql();
        return new PingDbContextPostgres(optionsBuilder.Options);
    }
}

public sealed class PingDbContextSqliteFactory : IDesignTimeDbContextFactory<PingDbContextSqlite>
{
    public PingDbContextSqlite CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PingDbContextSqlite>();
        optionsBuilder.UseSqlite();
        return new PingDbContextSqlite(optionsBuilder.Options);
    }
}
