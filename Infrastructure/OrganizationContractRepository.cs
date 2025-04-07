using Microsoft.Data.Sqlite;
using ContractApp.Models;
using System.Threading.Tasks;

namespace ContractApp.Infrastructure
{
    public static class OrganizationContractRepository
    {
        public static async Task<int> AddAsync(OrganizationContract contract)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO OrganizationContracts (Id, StudentId, OrganizationId)
                VALUES (@id, @studentId, @organizationId)";

            command.Parameters.AddWithValue("@id", contract.Id);
            command.Parameters.AddWithValue("@studentId", contract.StudentId);
            command.Parameters.AddWithValue("@organizationId", contract.OrganizationId);

            await command.ExecuteNonQueryAsync();
            return contract.Id;
        }
    }
}
