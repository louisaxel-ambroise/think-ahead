using ThinkAhead.Application;

namespace ThinkAhead.Controllers.Results;

public sealed class GameResult
{
    public required int Id { get; init; }
    public required string CurrentPlayer { get; set; }
    public required string Fen { get; init; }
    public GameState State { get; set; }
    public required GameResultMove[] Moves { get; init; } = [];
}

public sealed class GameResultMove
{
    public required char Piece { get; init; }
    public required string San { get; init; }
}