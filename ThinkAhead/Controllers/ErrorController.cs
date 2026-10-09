using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sqids;
using ThinkAhead.Controllers.Results;

namespace ThinkAhead.Controllers;

[Route("error")]
public sealed class ErrorController(SqidsEncoder<int> encoder) : Controller
{
    public IActionResult HandleError()
    {
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var parts = exceptionHandlerFeature?.Path.Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];

        if(exceptionHandlerFeature?.Error is KeyNotFoundException)
        {
            return new RedirectResult($"/", false, false);
        }
        else if (parts.Length >= 2 && parts[0] == "game" && encoder.Decode(parts[1]).Count == 1)
        {
            return new RedirectResult($"/game/{parts[1]}", false, false);
        }
        else
        {
            return new NotFoundObjectResult(new UnknownResult());
        }
    }
}
