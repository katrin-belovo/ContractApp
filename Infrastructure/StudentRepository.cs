using Microsoft.Data.Sqlite;
using ContractApp.Models;
using System.Threading.Tasks;

namespace ContractApp.Infrastructure
{
    public static class StudentRepository
    {
        public static async Task<Student> GetByPassportAsync(string series, string number)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT * FROM Students 
                WHERE PassportSeries = @series AND PassportNumber = @number";

            command.Parameters.AddWithValue("@series", series);
            command.Parameters.AddWithValue("@number", number);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Student
                {
                    Id = reader.GetInt32(0),
                    LastName = reader.GetString(1),
                    FirstName = reader.GetString(2),
                    MiddleName = reader.IsDBNull(3) ? null : reader.GetString(3),
                    PassportSeries = reader.GetString(4),
                    PassportNumber = reader.GetString(5),
                    Snils = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Inn = reader.IsDBNull(7) ? null : reader.GetString(7),
                    Phone = reader.IsDBNull(8) ? null : reader.GetString(8),
                    Address = reader.IsDBNull(9) ? null : reader.GetString(9),
                    BirthDate = reader.GetDateTime(10),
                    EducationBase = reader.GetString(11)
                };
            }
            return null;
        }

        public static async Task<int> AddAsync(Student student)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Students (
                    LastName, FirstName, MiddleName, 
                    PassportSeries, PassportNumber, 
                    Snils, Inn, Phone, Address, BirthDate, EducationBase
                )
                VALUES (
                    @lastName, @firstName, @middleName, 
                    @passportSeries, @passportNumber, 
                    @snils, @inn, @phone, @address, @birthDate, @educationBase
                );
                SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@lastName", student.LastName);
            command.Parameters.AddWithValue("@firstName", student.FirstName);
            command.Parameters.AddWithValue("@middleName", student.MiddleName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@passportSeries", student.PassportSeries);
            command.Parameters.AddWithValue("@passportNumber", student.PassportNumber);
            command.Parameters.AddWithValue("@snils", student.Snils ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@inn", student.Inn ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@phone", student.Phone ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@address", student.Address ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@birthDate", student.BirthDate);
            command.Parameters.AddWithValue("@educationBase", student.EducationBase);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }


        public static async Task UpdateAsync(Student student)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
        UPDATE Students 
        SET LastName = @lastName,
            FirstName = @firstName,
            MiddleName = @middleName,
            PassportSeries = @passportSeries,
            PassportNumber = @passportNumber,
            Snils = @snils,
            Inn = @inn,
            Phone = @phone,
            Address = @address,
            BirthDate = @birthDate,
            EducationBase = @educationBase
        WHERE Id = @id";

            command.Parameters.AddWithValue("@id", student.Id);
            command.Parameters.AddWithValue("@lastName", student.LastName);
            command.Parameters.AddWithValue("@firstName", student.FirstName);
            command.Parameters.AddWithValue("@middleName", student.MiddleName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@passportSeries", student.PassportSeries);
            command.Parameters.AddWithValue("@passportNumber", student.PassportNumber);
            command.Parameters.AddWithValue("@snils", student.Snils ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@inn", student.Inn ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@phone", student.Phone ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@address", student.Address ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@birthDate", student.BirthDate);
            command.Parameters.AddWithValue("@educationBase", student.EducationBase);

            await command.ExecuteNonQueryAsync();
        }
    }
}