using Microsoft.AspNetCore.Mvc;
using Sqids;
using ThinkAhead.Application.Data;
using ThinkAhead.Controllers.Results;

namespace ThinkAhead.Controllers;

[Route("")]
public sealed class HomeController(GameRepository repository, SqidsEncoder<int> encoder)
{
    public RootResult Root()
    {
        var activeGames = repository.LoadPage(5);

        return new RootResult
        {
            LatestGames = activeGames.Select(g => new GamePreview
            {
                Id = encoder.Encode(g.Id),
                Description = $"{g.CreatedOn:yyyy-MM-dd} - {g.State}"
            }).ToList()
        };
    }

    [Route("about")]
    public AboutResult About() => new();
    [Route("rules")]
    public RulesResult Rules() => new();
}
