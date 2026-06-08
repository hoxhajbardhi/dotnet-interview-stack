using Application.Features.Health;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    private readonly ISender _sender;

    public HealthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("ping")]
    public async Task<ActionResult<string>> Ping(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new PingQuery(), cancellationToken);
        return Ok(result);
    }

//     [HttpGet("throw/{kind}")]
// public async Task<ActionResult<string>> Throw(string kind, CancellationToken cancellationToken)
// {
//     var result = await _sender.Send(new ThrowTestQuery(kind), cancellationToken);
//     return Ok(result);
// }
}