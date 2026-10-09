using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace ThinkAhead.Views;

public static class ChessExtensions
{
    private static readonly Dictionary<char, string> PieceSymbols = new()
    {
        { 'p', "♟" }, { 'n', "♞" }, { 'b', "♝" }, { 'r', "♜" }, { 'q', "♛" }, { 'k', "♚" },
        { 'P', "♙" }, { 'N', "♘" }, { 'B', "♗" }, { 'R', "♖" }, { 'Q', "♕" }, { 'K', "♔" }
    };

    private static readonly Dictionary<char, string> PieceName = new()
    {
        { 'p', "Pawn" }, { 'n', "Knight" }, { 'b', "Bishop" }, { 'r', "Rook" }, { 'q', "Queen" }, { 'k', "King" }
    };

    public static string Sanitize(this string san)
    {
        return san.TrimEnd(['+', '#', 'Q', 'N', 'R', 'B', '=', '$']);
    }

    public static IHtmlContent ChessPiece(this IHtmlHelper helper, char piece, string currentPlayer)
    {
        var coloredPiece = currentPlayer == "white" ? char.ToUpper(piece) : char.ToLower(piece);
        return helper.Raw("<span class=\"piece\" title=\"" + PieceName.GetValueOrDefault(piece, string.Empty) + "\">" + PieceSymbols.GetValueOrDefault(coloredPiece, string.Empty) + "</span>");
    }

    public static IHtmlContent ChessBoard(this IHtmlHelper helper, string fen, params string[] affectedPositions)
    {

        var builder = new StringBuilder();
        var cols = fen.Split(' ')[0].Split('/');

        for (var col = 8; col > 0; col--)
        {
            var fenRow = cols[8 - col];
            for (var row = 0; row < 8; row++)
            {
                var extra = string.Empty;
                var className = (col + row) % 2 == 0 ? "light" : "dark";
                var piece = GetPiece(fenRow, row);

                if (affectedPositions.Contains($"{(char)('a' + row)}{col}"))
                {
                    className += " highlight";
                }
                if (row == 0)
                {
                    extra += $"col=\"{col}\"";
                }
                if (col == 1)
                {
                    extra += $"row=\"{(char)(row + 'a')}\"";
                }

                builder.Append($"<div class=\"{className}\" {extra}>{piece}</div>");
            }
        }

        return helper.Raw(builder.ToString());
    }

    private static string GetPiece(string row, int col)
    {
        var index = 0;

        for (var i = 0; i < row.Length; i++)
        {
            if (char.IsDigit(row[i]))
            {
                index += row[i] - '0';
            }
            else if (index == col)
            {
                return PieceSymbols[row[i]];
            }
            else 
            { 
                index++;
            }
        }

        return string.Empty;
    }
}
