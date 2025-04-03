using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContractApp.Models;
using System.IO;

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
                    ShortName = reader.GetString(3),
                    Level = reader.GetString(4)
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
            INSERT INTO Directions (Code, FullName, ShortName, Level)
            VALUES (@code, @full, @short, @level)";

            command.Parameters.AddWithValue("@code", direction.Code);
            command.Parameters.AddWithValue("@full", direction.FullName);
            command.Parameters.AddWithValue("@short", direction.ShortName);
            command.Parameters.AddWithValue("@level", direction.Level);

            await command.ExecuteNonQueryAsync();
            CreateDirectionFolder(direction.Code);
        }

        public static async Task UpdateAsync(Direction direction)
        {
            using var connection = new SqliteConnection($"Data Source=contracts.db");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
            UPDATE Directions 
            SET FullName = @full, ShortName = @short, Level = @level 
            WHERE Id = @id";

            command.Parameters.AddWithValue("@full", direction.FullName);
            command.Parameters.AddWithValue("@short", direction.ShortName);
            command.Parameters.AddWithValue("@id", direction.Id);
            command.Parameters.AddWithValue("@level", direction.Level);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task DeleteAsync(int id)
        {
            var direction = await GetByIdAsync(id);

            using var connection = new SqliteConnection($"Data Source=contracts.db");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Directions WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
            DeleteDirectionFolder(direction.Code);
        }

        private static async Task<Direction> GetByIdAsync(int id)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Directions WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Direction
                {
                    Id = reader.GetInt32(0),
                    Code = reader.GetString(1),
                    FullName = reader.GetString(2),
                    ShortName = reader.GetString(3),
                    Level = reader.GetString(4)
                };
            }
            return null;
        }

        private static void CreateDirectionFolder(string code)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Directions", code);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }


        private static void DeleteDirectionFolder(string code)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Directions", code);
            if (Directory.Exists(folderPath))
            {
                try
                {
                    Directory.Delete(folderPath, true);
                }
                catch
                {
                    // Логирование ошибки при необходимости
                }
            }
        }


        public static async Task<bool> HasAnyDirectionsAsync()
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Directions";

            return (long)await command.ExecuteScalarAsync() > 0;
        }

    }
}
