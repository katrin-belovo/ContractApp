using System;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Threading.Tasks;

namespace ContractApp.Infrastructure
{
    public static class DatabaseService
    {
        private const string DbPath = "contracts.db";

        public static async Task InitializeDatabaseAsync()
        {
            if (!File.Exists(DbPath) || !await TablesExistAsync())
            {
                await CreateDatabaseAsync();
            }
        }

        private static async Task<bool> TablesExistAsync()
        {
            try
            {
                using var connection = new SqliteConnection($"Data Source={DbPath}");
                await connection.OpenAsync();

                var checkTablesCommand = connection.CreateCommand();
                checkTablesCommand.CommandText = @"
                SELECT count(*) FROM sqlite_master 
                WHERE type='table' AND 
                (name='Directions' OR name='Groups')";

                return (long)await checkTablesCommand.ExecuteScalarAsync() == 2;
            }
            catch
            {
                return false;
            }
        }

        private static async Task CreateDatabaseAsync()
        {
            using var connection = new SqliteConnection($"Data Source={DbPath}");
            await connection.OpenAsync();

            var createTables = new[]
            {
            @"CREATE TABLE Directions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Code TEXT NOT NULL UNIQUE,
                FullName TEXT NOT NULL,
                ShortName TEXT NOT NULL CHECK(LENGTH(ShortName) <= 10),
                Level TEXT NOT NULL CHECK(Level IN ('ВО', 'СПО')))",

            @"CREATE TABLE Groups (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                DirectionId INTEGER NOT NULL,
                FOREIGN KEY(DirectionId) REFERENCES Directions(Id) ON DELETE CASCADE)"
        };

            foreach (var sql in createTables)
            {
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
