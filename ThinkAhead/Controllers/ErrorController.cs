using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ThinkAhead.Controllers.Results;

namespace ThinkAhead.Controllers;

[Route("error")]
public sealed class ErrorController : Controller
{
    public IActionResult HandleError()
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var parts = exceptionHandlerFeature?.Path.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];

        if(exceptionHandlerFeature?.Error is KeyNotFoundException)
        {
            return new RedirectResult($"/", false, false);
        }
        else if (parts.Length >= 2 && parts[0] == "game" && int.TryParse(parts[1], out var gameId))
        {
            return new RedirectResult($"/game/{gameId}", false, false);
        }
        else
        {
            return new NotFoundObjectResult(new UnknownResult());
        }
    }
}
