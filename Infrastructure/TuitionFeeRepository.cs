using ContractApp.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContractApp.Infrastructure
{
    public static class TuitionFeeRepository
    {
        public static async Task<List<TuitionFee>> GetAllAsync()
        {
            var fees = new List<TuitionFee>();

            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT t.Id, t.Year, t.DirectionId, t.Amount, 
                       d.Code, d.FullName 
                FROM TuitionFees t
                JOIN Directions d ON t.DirectionId = d.Id";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                fees.Add(new TuitionFee
                {
                    Id = reader.GetInt32(0),
                    Year = reader.GetInt32(1),
                    DirectionId = reader.GetInt32(2),
                    Amount = reader.GetDecimal(3),
                    Direction = new Direction
                    {
                        Code = reader.GetString(4),
                        FullName = reader.GetString(5)
                    }
                });
            }
            return fees;
        }

        public static async Task AddAsync(TuitionFee fee)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO TuitionFees (Year, DirectionId, Amount)
                VALUES (@year, @dirId, @amount)";

            command.Parameters.AddWithValue("@year", fee.Year);
            command.Parameters.AddWithValue("@dirId", fee.DirectionId);
            command.Parameters.AddWithValue("@amount", fee.Amount);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM TuitionFees WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();
        }
    }
}