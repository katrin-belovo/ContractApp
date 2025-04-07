using Microsoft.Data.Sqlite;
using ContractApp.Models;
using System.Threading.Tasks;

namespace ContractApp.Infrastructure
{
    public static class OrganizationRepository
    {
        public static async Task<int> AddAsync(Organization organization)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Organizations (
                    Name, Inn, Kpp, Address, Phone
                )
                VALUES (
                    @name, @inn, @kpp, @address, @phone
                );
                SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@name", organization.Name);
            command.Parameters.AddWithValue("@inn", organization.Inn);
            command.Parameters.AddWithValue("@kpp", organization.Kpp ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@address", organization.Address ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@phone", organization.Phone ?? (object)DBNull.Value);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
    }
}