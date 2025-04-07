using Microsoft.Data.Sqlite;
using ContractApp.Models;
using System.Threading.Tasks;

namespace ContractApp.Infrastructure
{
    public static class TrilateralContractRepository
    {
        public static async Task<int> AddAsync(TrilateralContract contract)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO TrilateralContracts (Id, StudentId, RepresentativeId)
                VALUES (@id, @studentId, @representativeId)";

            command.Parameters.AddWithValue("@id", contract.Id);
            command.Parameters.AddWithValue("@studentId", contract.StudentId);
            command.Parameters.AddWithValue("@representativeId", contract.RepresentativeId);

            await command.ExecuteNonQueryAsync();
            return contract.Id;
        }
    }
}