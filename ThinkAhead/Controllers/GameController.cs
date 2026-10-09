using Microsoft.AspNetCore.Mvc;
using ThinkAhead.Application;
using ThinkAhead.Controllers.Requests;
using ThinkAhead.Application.Data;
using ThinkAhead.Views;
using ThinkAhead.Controllers.Results;
using Sqids;

namespace ThinkAhead.Controllers;

[Controller]
[Route("game")]
public class GameController(GameRepository gameRepository, SqidsEncoder<int> encoder)
{
    [HttpGet("{id}")]
    public IActionResult LoadGame(string id)
    {
        var gameId = encoder.Decode(id).Single();
        var game = gameRepository.Load(gameId);

        return new OkObjectResult(new GameResult
        {
            Id = id,
            Fen = game.Fen,
            CurrentPlayer = game.CurrentPlayer,
            State = game.State,
            Moves = [.. game.Hand.Select(m => new GameResultMove { Piece = m.Piece, San = m.San })]
        });
    }

    [HttpGet("{id}/preview/{san}")]
    public PreviewResult PreviewMove(string id, string san)
    {
        var gameId = encoder.Decode(id).Single();
        var game = gameRepository.Load(gameId);
        var projection = game.PreviewMove(san);

        return new PreviewResult
        {
            Id = id,
            Fen = projection.Fen,
            CurrentPlayer = game.CurrentPlayer,
            SelectedMove = new SelectedMove
            {
                Piece = game.Hand.Single(m => m.San == san).Piece,
                San = game.Hand.Single(m => m.San == san).San,
                AffectedPositions = [game.Hand.Single(m => m.San == san).FromPosition, game.Hand.Single(m => m.San == san).ToPosition]
            },
            Moves = [.. game.Hand.Select(m => new PreviewMove { Piece = m.Piece, San = m.San })],
            NextMoves = [.. projection.Hand.Select(m => new PreviewMove { Piece = m.Piece, San = m.San.Sanitize() })]
        };
    }

    [HttpPost]
    public IActionResult Create()
    {
        var game = new Game(0, DateTimeOffset.UtcNow);
        var gameId = gameRepository.Insert(game);

        return new RedirectResult($"/game/{encoder.Encode(gameId)}", false, false);
    }

    [HttpPost("{id}/play")]
    public IActionResult Play(string id, [FromForm] PlayMoveRequest request)
    {
        var gameId = encoder.Decode(id).Single();
        var game = gameRepository.Load(gameId);

        game.PlayMove(request.San);
        gameRepository.Save(game);

        return new RedirectResult($"/game/{encoder.Encode(gameId)}", false, false);
    }
}