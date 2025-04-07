using Microsoft.Data.Sqlite;
using ContractApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContractApp.Infrastructure
{
    public static class ContractRepository
    {
        public static async Task<List<Contract>> GetAllAsync()
        {
            var contracts = new List<Contract>();

            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT c.Id, c.Number, c.CreationDate, c.ConclusionDate, 
                       c.TerminationDate, c.Status
                FROM Contracts c
                ORDER BY c.CreationDate DESC";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                contracts.Add(new Contract
                {
                    Id = reader.GetInt32(0),
                    Number = reader.GetString(1),
                    CreationDate = reader.GetDateTime(2),
                    ConclusionDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    TerminationDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    Status = reader.GetString(5)
                });
            }

            return contracts;
        }

        public static async Task<int> CreateBaseContractAsync(Contract contract)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
        INSERT INTO Contracts (Number, CreationDate, Status)
        VALUES (@number, @creationDate, @status);
        SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@number", contract.Number);
            command.Parameters.AddWithValue("@creationDate", contract.CreationDate);
            command.Parameters.AddWithValue("@status", contract.Status);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public static async Task UpdateContractStatusAsync(int id, string status)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Contracts 
                SET Status = @status
                WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@status", status);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task ConcludeContractAsync(int id)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Contracts 
                SET ConclusionDate = @date, 
                    Status = 'Заключен'
                WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@date", DateTime.Now);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task TerminateContractAsync(int id)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Contracts 
                SET TerminationDate = @date, 
                    Status = 'Расторгнут'
                WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@date", DateTime.Now);

            await command.ExecuteNonQueryAsync();
        }
    }
}
