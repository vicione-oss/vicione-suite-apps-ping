using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ping.Backend.DbContext;
using Ping.Public.Contracts;

namespace Ping.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PingController(IPingDbContext dbContext, ILogger<PingController> logger) : ControllerBase
{
    private readonly ILogger<PingController> _logger = logger;

    [HttpGet("{pingId}")]
    public ActionResult GetPing(string pingId)
    {
        try
        {
            if (Guid.TryParse(pingId, out var guid))
            {
                var ping = dbContext.Pings.FirstOrDefaultAsync(k => k.Id == guid);
                return ping is null ? NotFound(guid) : Ok(ping);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get ping {PingId}", pingId);
            return StatusCode(500);
        }

        return NotFound(pingId);
    }

    [HttpGet]
    public async Task<ActionResult<List<PingResult>>> GetPings()
        => Ok(await dbContext.Pings.ToListAsync());
}
