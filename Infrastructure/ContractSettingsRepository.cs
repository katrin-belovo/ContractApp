using System.Collections.Generic;
using System.Threading.Tasks;
using ContractApp.Models;
using Microsoft.Data.Sqlite;

namespace ContractApp.Infrastructure
{
    public static class ContractSettingsRepository
    {
        public static async Task<List<ContractSettings>> GetAllAsync()
        {
            var settingsList = new List<ContractSettings>();

            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ContractSettings";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                settingsList.Add(new ContractSettings
                {
                    Id = reader.GetInt32(0),
                    Position = reader.GetString(1),
                    FullName = reader.GetString(2),
                    ProxyNumber = reader.GetString(3),
                    ProxyDate = reader.GetDateTime(4),
                    IsActive = reader.GetBoolean(5)
                });
            }

            return settingsList;
        }

        public static async Task AddAsync(ContractSettings settings)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Деактивируем все существующие записи
                var deactivateCommand = connection.CreateCommand();
                deactivateCommand.CommandText = "UPDATE ContractSettings SET IsActive = 0";
                await deactivateCommand.ExecuteNonQueryAsync();

                // Добавляем новую запись
                var addCommand = connection.CreateCommand();
                addCommand.CommandText = @"
                    INSERT INTO ContractSettings (Position, FullName, ProxyNumber, ProxyDate, IsActive)
                    VALUES (@position, @fullName, @proxyNumber, @proxyDate, 1)";
                addCommand.Parameters.AddWithValue("@position", settings.Position);
                addCommand.Parameters.AddWithValue("@fullName", settings.FullName);
                addCommand.Parameters.AddWithValue("@proxyNumber", settings.ProxyNumber);
                addCommand.Parameters.AddWithValue("@proxyDate", settings.ProxyDate);

                await addCommand.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public static async Task UpdateAsync(ContractSettings settings)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                if (settings.IsActive)
                {
                    var deactivateCommand = connection.CreateCommand();
                    deactivateCommand.CommandText = "UPDATE ContractSettings SET IsActive = 0 WHERE Id != @id";
                    deactivateCommand.Parameters.AddWithValue("@id", settings.Id);
                    await deactivateCommand.ExecuteNonQueryAsync();
                }

                var updateCommand = connection.CreateCommand();
                updateCommand.CommandText = @"
                    UPDATE ContractSettings 
                    SET Position = @position,
                        FullName = @fullName,
                        ProxyNumber = @proxyNumber,
                        ProxyDate = @proxyDate,
                        IsActive = @isActive
                    WHERE Id = @id";
                updateCommand.Parameters.AddWithValue("@position", settings.Position);
                updateCommand.Parameters.AddWithValue("@fullName", settings.FullName);
                updateCommand.Parameters.AddWithValue("@proxyNumber", settings.ProxyNumber);
                updateCommand.Parameters.AddWithValue("@proxyDate", settings.ProxyDate);
                updateCommand.Parameters.AddWithValue("@isActive", settings.IsActive);
                updateCommand.Parameters.AddWithValue("@id", settings.Id);

                await updateCommand.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public static async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM ContractSettings WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }

        public static async Task ActivateAsync(int id)
        {
            using var connection = new SqliteConnection("Data Source=contracts.db");
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var deactivateCommand = connection.CreateCommand();
                deactivateCommand.CommandText = "UPDATE ContractSettings SET IsActive = 0";
                await deactivateCommand.ExecuteNonQueryAsync();

                var activateCommand = connection.CreateCommand();
                activateCommand.CommandText = "UPDATE ContractSettings SET IsActive = 1 WHERE Id = @id";
                activateCommand.Parameters.AddWithValue("@id", id);
                await activateCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


    }
}