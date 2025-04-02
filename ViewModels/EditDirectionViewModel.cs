using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ContractApp.Infrastructure;
using ContractApp.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Data.Sqlite;

namespace ContractApp.ViewModels
{
    public class EditDirectionViewModel : ViewModelBase
    {
        private Direction _direction;
        private string _errorMessage;
        private bool _hasError;
        public bool IsEditing => _direction.Id != 0;
        public EditDirectionViewModel(Direction direction)
        {
            _direction = direction;
            SaveCommand = new RelayCommand(async () => await SaveAsync());
        }

        public string WindowTitle => _direction.Id == 0 ? "Новое направление" : "Редактирование направления";

        public string Code
        {
            get => _direction.Code;
            set
            {
                _direction.Code = value;
                RaisePropertyChanged();
            }
        }

        public string FullName
        {
            get => _direction.FullName;
            set
            {
                _direction.FullName = value;
                RaisePropertyChanged();
            }
        }

        public string ShortName
        {
            get => _direction.ShortName;
            set
            {
                _direction.ShortName = value;
                RaisePropertyChanged();
            }
        }

        public string Level
        {
            get => _direction.Level;
            set
            {
                _direction.Level = value;
                RaisePropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                RaisePropertyChanged();
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                RaisePropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        private async Task SaveAsync()
        {
            try
            {
                if (!Validate())
                    return;

                if (_direction.Id == 0)
                    await DirectionRepository.AddAsync(_direction);
                else
                    await DirectionRepository.UpdateAsync(_direction);

                CloseWindow();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                ErrorMessage = "Направление с таким кодом уже существует!";
                HasError = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
                HasError = true;
            }
        }

        private bool Validate()
        {
            var errors = new List<string>();


            if (string.IsNullOrWhiteSpace(FullName))
                errors.Add("Укажите полное название");

            if (string.IsNullOrWhiteSpace(ShortName) || ShortName.Length > 10)
                errors.Add("Короткое название должно быть 1-10 символов");

            if (string.IsNullOrEmpty(Level))
                errors.Add("Выберите уровень образования");

            if (!new[] { "ВО", "СПО" }.Contains(Level))
                errors.Add("Некорректный уровень образования");

            if (errors.Any())
            {
                ErrorMessage = string.Join("\n", errors);
                HasError = true;
                return false;
            }

            HasError = false;
            return true;
        }

        private void CloseWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.DialogResult = true;
                        window.Close();
                        break;
                    }
                }
            });
        }
    }
}
