using Microsoft.Data.Sqlite;
using ContractApp.Models;
using System.Threading.Tasks;

namespace ContractApp.Infrastructure
{
    public static class BilateralContractRepository
    {
        public static async Task<int> AddAsync(BilateralContract contract)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO BilateralContracts (Id, StudentId)
                VALUES (@id, @studentId)";

            command.Parameters.AddWithValue("@id", contract.Id);
            command.Parameters.AddWithValue("@studentId", contract.StudentId);

            await command.ExecuteNonQueryAsync();
            return contract.Id;
        }
    }
}