using Chess;

namespace ThinkAhead.Application;

public sealed class Game(int id, string pgn = "")
{
    public int Id => id;
    public string Fen => _board.ToFen();
    public string Pgn => _board.ToPgn();
    public string CurrentPlayer => _board.Turn == PieceColor.White ? "white" : "black";
    public GameState State => _board.IsEndGame ? GetEndGameState(_board.EndGame) : GameState.Active;
    public List<Move> Hand => GetAvailableMoves();

    private readonly ChessBoard _board = ChessBoard.LoadFromPgn(pgn, AutoEndgameRules.All);

    public Game PreviewMove(string move)
    {
        var projection = new Game(Id, _board.ToPgn());
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
    }

    private List<Move> GetAvailableMoves()
    {
        var seed = _board.ExecutedMoves.Sum(m => m.San!.GetHashCode() % (1E9 / _board.ExecutedMoves.Count)) % 1E9 + Id;
        var random = new Random((int)seed);

        var validMoves = _board.Moves().OrderBy(p => p.OriginalPosition.ToString());
        var selectedMoves = validMoves.OrderBy(_ => random.Next()).Take(5).Select(move => new Move
        {
            San = move.San ?? "?",
            Piece = GetPieceChar(move.Piece),
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

    private static char GetPieceChar(Piece piece)
    {
        return piece.Color.AsChar switch
        {
            'b' => char.ToLower(piece.Type.AsChar),
            'w' => char.ToUpper(piece.Type.AsChar),
            _ => '?'
        };
    }
}
