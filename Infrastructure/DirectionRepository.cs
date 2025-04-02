using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContractApp.Models;

namespace ContractApp.Infrastructure
{
    public static class DirectionRepository
    {
        public static async Task<List<Direction>> GetAllAsync()
        {
            var directions = new List<Direction>();

            using var connection = new SqliteConnection($"Data Source=contracts.db");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Directions";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                directions.Add(new Direction
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1),
                    FullName = reader.GetString(2),
                    ShortName = reader.GetString(3)
                });
            }
            return directions;
        }

        public static async Task AddAsync(Direction direction)
        {
            using var connection = new SqliteConnection($"Data Source=contracts.db");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Directions (Code, FullName, ShortName)
            VALUES (@code, @full, @short)";

            command.Parameters.AddWithValue("@code", direction.Code);
            command.Parameters.AddWithValue("@full", direction.FullName);
            command.Parameters.AddWithValue("@short", direction.ShortName);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task UpdateAsync(Direction direction)
        {
            using var connection = new SqliteConnection($"Data Source=contracts.db");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
            UPDATE Directions 
            SET Code = @code, FullName = @full, ShortName = @short 
            WHERE Id = @id";

            command.Parameters.AddWithValue("@code", direction.Code);
            command.Parameters.AddWithValue("@full", direction.FullName);
            command.Parameters.AddWithValue("@short", direction.ShortName);
            command.Parameters.AddWithValue("@id", direction.Id);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection($"Data Source=contracts.db");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Directions WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
