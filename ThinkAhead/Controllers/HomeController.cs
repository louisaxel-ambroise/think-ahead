using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ThinkAhead.Controllers.Results;

namespace ThinkAhead.Controllers;

[Route("")]
public sealed class HomeController
{
    public RootResult Root()
    {
        return new RootResult
        {
            Id = Random.Shared.Next()
        };
    }

    [Route("about")]
    public AboutResult About() => new();
    [Route("rules")]
    public RulesResult Rules() => new();
}
