using ContractApp.Infrastructure;
using ContractApp.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ContractApp.Views
{
    public partial class EditContractSettingsWindow : Window
    {
        private readonly ContractSettings _settings;

        public EditContractSettingsWindow(ContractSettings settings = null)
        {
            InitializeComponent();
            _settings = settings ?? new ContractSettings { IsActive = true };
            DataContext = _settings;
            PositionCombo.ItemsSource = new[] { "Директор", "И.О. Директора" };

            if (_settings.Id == 0)
            {
                IsActiveCheckBox.Visibility = Visibility.Collapsed;
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_settings.Position))
                {
                    ErrorText.Text = "Выберите должность!";
                    return;
                }
                if (string.IsNullOrWhiteSpace(_settings.FullName))
                {
                    ErrorText.Text = "Введите ФИО!";
                    return;
                }
                if (string.IsNullOrWhiteSpace(_settings.ProxyNumber))
                {
                    ErrorText.Text = "Введите номер доверенности!";
                    return;
                }
                if (_settings.ProxyDate > DateTime.Now)
                {
                    ErrorText.Text = "Дата доверенности не может быть в будущем!";
                    return;
                }

                if (_settings.Id == 0)
                {
                    await ContractSettingsRepository.AddAsync(_settings);
                }
                else
                {
                    await ContractSettingsRepository.UpdateAsync(_settings);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void DatePicker_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            ErrorText.Text = $"Дата должна быть между 01.01.2000 и {DateTime.Now:dd.MM.yyyy}";
            ((DatePicker)sender).SelectedDate = DateTime.Today;
        }

        private void DatePicker_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var datePicker = (DatePicker)sender;
            if (DateTime.TryParse(e.Text, CultureInfo.CurrentCulture, DateTimeStyles.None, out _)) return;

            e.Handled = true;
            ShowNotification("Некорректный формат даты! Используйте дд.мм.гггг", true);
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var datePicker = (DatePicker)sender;
            if (datePicker.SelectedDate == null) return;

            var minDate = new DateTime(2000, 1, 1);
            var maxDate = DateTime.Today;

            if (datePicker.SelectedDate < minDate)
            {
                datePicker.SelectedDate = minDate;
                ShowNotification("Дата автоматически изменена на минимальную (01.01.2000)", true);
            }
            else if (datePicker.SelectedDate > maxDate)
            {
                datePicker.SelectedDate = maxDate;
                ShowNotification("Дата автоматически изменена на текущую", true);
            }
        }

        private void ShowNotification(string message, bool isError)
        {
            if (isError)
            {
                ErrorText.Text = message;
            }
            else
            {
                ErrorText.Text = string.Empty;
            }

            // Автоочистка через 3 секунды
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += (s, e) =>
            {
                ErrorText.Text = string.Empty;
                timer.Stop();
            };
            timer.Start();
        }
    }
}
