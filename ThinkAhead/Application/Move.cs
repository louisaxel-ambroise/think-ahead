namespace ThinkAhead.Application;

public class Move
{
    public required string San { get; set; }
    public required char Piece { get; set; }
    public required string FromPosition { get; set; }
    public required string ToPosition { get; set; }
}
