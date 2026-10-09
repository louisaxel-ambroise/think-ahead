using Chess;

namespace ThinkAhead.Application;

public sealed class Game(int id, DateTimeOffset createdOn, string pgn = "")
{
    public int Id => id;
    public DateTimeOffset CreatedOn => createdOn;
    public string Fen => _board.ToFen();
    public string Pgn => _board.ToPgn();
    public string CurrentPlayer => _board.Turn == PieceColor.White ? "white" : "black";
    public GameState State => _board.IsEndGame ? GetEndGameState(_board.EndGame) : GameState.Active;
    public List<Move> Hand => _hand ??= GetAvailableMoves();

    private readonly ChessBoard _board = ChessBoard.LoadFromPgn(pgn, AutoEndgameRules.All);
    private List<Move>? _hand;

    public Game PreviewMove(string move)
    {
        var projection = new Game(Id, CreatedOn, _board.ToPgn());
        projection.PlayMove(move);

        return projection;
    }

    public void PlayMove(string move)
    {
        if (!Hand.Any(m => m.San == move))
        {
            throw new ArgumentException("Invalid move");
        }

        _board.Move(move);
        _hand = GetAvailableMoves();
    }

    private List<Move> GetAvailableMoves()
    {
        var seed = Id + (_board.MoveIndex << 2) + _board.ExecutedMoves.LastOrDefault()?.San?.ToCharArray().Aggregate(1, (i, x) => (i << 2) + x) ?? 0;
        var random = new Random(seed);
        var moves = _board.Moves().OrderBy(p => p.San);
        var selectedMoves = moves.OrderBy(_ => random.Next()).Take(5).Select(move => new Move
        {
            San = move.San ?? "?",
            Piece = move.Piece.Type.AsChar,
            FromPosition = move.OriginalPosition.ToString(),
            ToPosition = move.NewPosition.ToString()
        }).ToList();

        return selectedMoves;
    }

    private static GameState GetEndGameState(EndGameInfo? endGame)
    {
        return endGame?.EndgameType switch
        {
            EndgameType.Stalemate or EndgameType.InsufficientMaterial or EndgameType.FiftyMoveRule or EndgameType.Repetition or EndgameType.DrawDeclared => GameState.Draw,
            EndgameType.Checkmate or EndgameType.Resigned or EndgameType.Timeout => endGame.WonSide == PieceColor.White ? GameState.WhiteWinning : GameState.BlackWinning,
            _ => GameState.Active
        };
    }
}
