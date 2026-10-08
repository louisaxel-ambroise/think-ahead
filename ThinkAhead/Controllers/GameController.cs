using Microsoft.AspNetCore.Mvc;
using ThinkAhead.Application;
using ThinkAhead.Controllers.Requests;
using ThinkAhead.Application.Data;
using ThinkAhead.Views;
using ThinkAhead.Controllers.Results;

namespace ThinkAhead.Controllers;

[Controller]
[Route("game")]
public class GameController(GameRepository gameRepository)
{
    [HttpGet("{id}")]
    public IActionResult LoadGame(int id)
    {
        var game = gameRepository.Load(id);

        return new OkObjectResult(new GameResult
        {
            Id = game.Id,
            Fen = game.Fen,
            CurrentPlayer = game.CurrentPlayer,
            State = game.State,
            Moves = [.. game.Hand.Select(m => new GameResultMove { Piece = m.Piece, San = m.San })]
        });
    }

    [HttpGet("{id}/preview/{san}")]
    public PreviewResult PreviewMove(int id, string san)
    {
        var game = gameRepository.Load(id);
        var projection = game.PreviewMove(san);

        return new PreviewResult
        {
            Id = projection.Id,
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
        var game = new Game(Random.Shared.Next());
        gameRepository.Insert(game);

        return new RedirectResult($"/game/{game.Id}", false, false);
    }

    [HttpPost("{id}/play")]
    public IActionResult Play(int id, [FromForm] PlayMoveRequest request)
    {
        var game = gameRepository.Load(id);

        game.PlayMove(request.San);
        gameRepository.Save(game);

        return new RedirectResult($"/game/{id}", false, false);
    }
}