namespace ThinkAhead.Controllers.Results;

public sealed class PreviewResult
{
    public required string Id { get; init; }
    public required string Fen { get; init; }
    public required string CurrentPlayer { get; set; }
    public required SelectedMove SelectedMove { get; set; }
    public required PreviewMove[] Moves { get; init; } = [];
    public required PreviewMove[] NextMoves { get; init; } = [];
}

public sealed class SelectedMove
{
    public required char Piece { get; init; }
    public required string[] AffectedPositions { get; init; }
    public required string San { get; init; }
}

public sealed class PreviewMove
{
    public required char Piece { get; init; }
    public required string San { get; init; }
}