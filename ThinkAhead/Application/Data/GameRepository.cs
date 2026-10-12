using Microsoft.Data.Sqlite;

namespace ThinkAhead.Application.Data;

public sealed class GameRepository(string connectionString)
{   
    public Game Load(int id)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, CreatedOn, Seed, Pgn FROM Games WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            throw new KeyNotFoundException("Game not found");
        }

        return new Game(reader.GetInt32(0), DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(1)), reader.GetInt32(2), reader.GetString(3));
    }

    public int Insert(Game game)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var insert = connection.CreateCommand();
        insert.CommandText = "INSERT INTO Games(CreatedOn, Pgn) VALUES (@CreatedOn, @Pgn)";
        insert.Parameters.AddWithValue("@CreatedOn", game.CreatedOn.ToUnixTimeSeconds());
        insert.Parameters.AddWithValue("@Pgn", game.Pgn);
        insert.ExecuteNonQuery();

        var query = connection.CreateCommand();
        query.CommandText = "SELECT last_insert_rowid();";
        var reader = query.ExecuteReader();
        reader.Read();
        
        return reader.GetInt32(0);
    }

    public void Save(Game game)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Games SET Pgn = @Pgn WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", game.Id);
        command.Parameters.AddWithValue("@Pgn", game.Pgn);
        command.ExecuteNonQuery();
    }

    public IEnumerable<Game> LoadPage(int pageSize)
    {
        
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, CreatedOn, Seed, Pgn FROM Games ORDER BY CreatedOn DESC LIMIT @PageSize";
        command.Parameters.AddWithValue("@PageSize", pageSize);

        using var reader = command.ExecuteReader();

        var result = new List<Game>();

        while (reader.Read()) 
        {
            result.Add(new Game(reader.GetInt32(0), DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(1)), reader.GetInt32(2), reader.GetString(3)));
        }

        return result;
    }
}
