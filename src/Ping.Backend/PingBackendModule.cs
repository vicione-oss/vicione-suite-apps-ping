using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ping.Backend.DbContext;
using Sdk.Backend.Extensions;
using Sdk.Backend.Modules;

namespace Ping.Backend;

public sealed class PingBackendModule : BackendModule
{
    public override IModuleInitializer ModuleInitializer => new DefaultModuleInitializer<IPingDbContext>();
    public override void ConfigureServices(IServiceCollection services, IConfiguration config, IMvcBuilder builder)
        => services.AddDynamicDbContext<IPingDbContext, PingDbContextSqlite, PingDbContextPostgres>(this);
}
