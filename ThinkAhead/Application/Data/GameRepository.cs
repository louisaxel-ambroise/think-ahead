using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace ThinkAhead.Application.Data;

public sealed class GameRepository
{
    public Game Load(int id)
    {
        using var connection = new SqliteConnection("Data Source=Chess.db");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Pgn FROM Games WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            throw new KeyNotFoundException("Game not found");
        }

        return new Game(reader.GetInt32(0), reader.GetString(1));
    }

    public void Insert(Game game)
    {
        using var connection = new SqliteConnection("Data Source=Chess.db");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Games(Id, Pgn) VALUES (@Id, @Pgn)";
        command.Parameters.AddWithValue("@Id", game.Id);
        command.Parameters.AddWithValue("@Pgn", game.Pgn);
        command.ExecuteNonQuery();
    }

    public void Save(Game game)
    {
        using var connection = new SqliteConnection("Data Source=Chess.db");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Games SET Pgn = @Pgn WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", game.Id);
        command.Parameters.AddWithValue("@Pgn", game.Pgn);
        command.ExecuteNonQuery();
    }
}
