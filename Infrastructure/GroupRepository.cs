using System;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using ContractApp.Models;
namespace ContractApp.Infrastructure
{
    public static class GroupRepository
    {
        public static async Task<List<Group>> GetAllAsync()
        {
            var groups = new List<Group>();

            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
            SELECT g.Id, g.Name, g.DirectionId, d.Code, d.FullName 
            FROM Groups g
            JOIN Directions d ON g.DirectionId = d.Id";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                groups.Add(new Group
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    DirectionId = reader.GetInt32(2),
                    Direction = new Direction
                    {
                        Code = reader.GetString(3),
                        FullName = reader.GetString(4)
                    }
                });
            }
            return groups;
        }

        public static async Task AddAsync(Group group)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Groups (Name, DirectionId)
            VALUES (@name, @dirId)";

            command.Parameters.AddWithValue("@name", group.Name);
            command.Parameters.AddWithValue("@dirId", group.DirectionId);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Groups WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task<bool> HasGroupsAsync(int directionId)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Groups WHERE DirectionId = @directionId";
            command.Parameters.AddWithValue("@directionId", directionId);

            var count = (long)await command.ExecuteScalarAsync();
            return count > 0;
        }
    }
}
