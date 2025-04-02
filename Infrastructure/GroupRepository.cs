using System;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
namespace ContractApp.Infrastructure
{
    public static class GroupRepository
    {
        public static async Task<bool> HasGroupsAsync(int directionId)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Groups WHERE DirectionId = @directionId";
            command.Parameters.AddWithValue("@directionId", directionId);

            return (long)await command.ExecuteScalarAsync() > 0;
        }
    }
}
