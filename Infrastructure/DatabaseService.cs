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
                FOREIGN KEY(DirectionId) REFERENCES Directions(Id) ON DELETE CASCADE)",
            @"CREATE TABLE TuitionFees (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Year INTEGER NOT NULL,
                DirectionId INTEGER NOT NULL,
                Amount REAL NOT NULL,
                FOREIGN KEY(DirectionId) REFERENCES Directions(Id))",
            @"CREATE TABLE ContractSettings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Position TEXT NOT NULL,
                FullName TEXT NOT NULL,
                ProxyNumber TEXT NOT NULL,
                ProxyDate DATETIME NOT NULL,
                IsActive INTEGER NOT NULL CHECK (IsActive IN (0, 1)))",
            @"CREATE TABLE Contracts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Number TEXT NOT NULL UNIQUE,
                    CreationDate TEXT NOT NULL,
                    ConclusionDate TEXT,
                    TerminationDate TEXT,
                    Status TEXT NOT NULL
                )",

                @"CREATE TABLE BilateralContracts (
                    Id INTEGER PRIMARY KEY,
                    StudentId INTEGER NOT NULL,
                    FOREIGN KEY(Id) REFERENCES Contracts(Id),
                    FOREIGN KEY(StudentId) REFERENCES Students(Id)
                )",

                @"CREATE TABLE TrilateralContracts (
                    Id INTEGER PRIMARY KEY,
                    StudentId INTEGER NOT NULL,
                    RepresentativeId INTEGER NOT NULL,
                    FOREIGN KEY(Id) REFERENCES Contracts(Id),
                    FOREIGN KEY(StudentId) REFERENCES Students(Id),
                    FOREIGN KEY(RepresentativeId) REFERENCES Representatives(Id)
                )",

                @"CREATE TABLE OrganizationContracts (
                    Id INTEGER PRIMARY KEY,
                    StudentId INTEGER NOT NULL,
                    OrganizationId INTEGER NOT NULL,
                    FOREIGN KEY(Id) REFERENCES Contracts(Id),
                    FOREIGN KEY(StudentId) REFERENCES Students(Id),
                    FOREIGN KEY(OrganizationId) REFERENCES Organizations(Id)
                )",

                @"CREATE TABLE Students (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LastName TEXT NOT NULL,
                    FirstName TEXT NOT NULL,
                    MiddleName TEXT,
                    PassportSeries TEXT NOT NULL,
                    PassportNumber TEXT NOT NULL,
                    Snils TEXT,
                    Inn TEXT,
                    Phone TEXT,
                    Address TEXT,
                    BirthDate TEXT NOT NULL,
                    EducationBase TEXT NOT NULL
                )",

                @"CREATE TABLE Representatives (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LastName TEXT NOT NULL,
                    FirstName TEXT NOT NULL,
                    MiddleName TEXT,
                    PassportSeries TEXT NOT NULL,
                    PassportNumber TEXT NOT NULL,
                    Inn TEXT,
                    Snils TEXT,
                    Phone TEXT
                )",

                @"CREATE TABLE Organizations (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Inn TEXT NOT NULL,
                    Kpp TEXT,
                    Address TEXT,
                    Phone TEXT
                )"
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
