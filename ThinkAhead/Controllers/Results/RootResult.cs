namespace ThinkAhead.Controllers.Results;

public class RootResult
{
    public required List<GamePreview> LatestGames { get; set; } = [];
}

public class GamePreview
{
    public required string Id { get; set; }
    public required string Description { get; set; }
}
