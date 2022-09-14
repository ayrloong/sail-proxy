using Microsoft.AspNetCore.Mvc;
using Sail.Kubernetes.Gateway.Dispatching;

namespace  Sail.Kubernetes.Gateway.Controllers;

[Route("api/dispatch")]
[ApiController]
public class DispatchController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public DispatchController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("/api/dispatch")]
    public Task<IActionResult> WatchAsync()
    {
        return Task.FromResult<IActionResult>(new DispatchActionResult(_dispatcher));
    }
}
