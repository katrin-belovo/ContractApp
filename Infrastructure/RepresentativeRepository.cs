using System;
using Microsoft.Data.Sqlite;
using ContractApp.Models;
using System.Threading.Tasks;

namespace ContractApp.Infrastructure
{
    public static class RepresentativeRepository
    {
        public static async Task<Representative> GetByPassportAsync(string series, string number)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Representatives WHERE PassportSeries = @series AND PassportNumber = @number";
            command.Parameters.AddWithValue("@series", series);
            command.Parameters.AddWithValue("@number", number);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Representative
                {
                    Id = reader.GetInt32(0),
                    LastName = reader.GetString(1),
                    FirstName = reader.GetString(2),
                    MiddleName = reader.IsDBNull(3) ? null : reader.GetString(3),
                    PassportSeries = reader.GetString(4),
                    PassportNumber = reader.GetString(5),
                    Inn = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Snils = reader.IsDBNull(7) ? null : reader.GetString(7),
                    Phone = reader.IsDBNull(8) ? null : reader.GetString(8)
                };
            }
            return null;
        }

        public static async Task<int> AddAsync(Representative representative)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Representatives (
                    LastName, FirstName, MiddleName, 
                    PassportSeries, PassportNumber, 
                    Inn, Snils, Phone
                )
                VALUES (
                    @lastName, @firstName, @middleName, 
                    @passportSeries, @passportNumber, 
                    @inn, @snils, @phone
                );
                SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@lastName", representative.LastName);
            command.Parameters.AddWithValue("@firstName", representative.FirstName);
            command.Parameters.AddWithValue("@middleName", representative.MiddleName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@passportSeries", representative.PassportSeries);
            command.Parameters.AddWithValue("@passportNumber", representative.PassportNumber);
            command.Parameters.AddWithValue("@inn", representative.Inn ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@snils", representative.Snils ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@phone", representative.Phone ?? (object)DBNull.Value);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public static async Task UpdateAsync(Representative representative)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Representatives 
                SET LastName = @lastName,
                    FirstName = @firstName,
                    MiddleName = @middleName,
                    PassportSeries = @passportSeries,
                    PassportNumber = @passportNumber,
                    Inn = @inn,
                    Snils = @snils,
                    Phone = @phone
                WHERE Id = @id";

            command.Parameters.AddWithValue("@id", representative.Id);
            command.Parameters.AddWithValue("@lastName", representative.LastName);
            command.Parameters.AddWithValue("@firstName", representative.FirstName);
            command.Parameters.AddWithValue("@middleName", representative.MiddleName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@passportSeries", representative.PassportSeries);
            command.Parameters.AddWithValue("@passportNumber", representative.PassportNumber);
            command.Parameters.AddWithValue("@inn", representative.Inn ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@snils", representative.Snils ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@phone", representative.Phone ?? (object)DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }
    }
}