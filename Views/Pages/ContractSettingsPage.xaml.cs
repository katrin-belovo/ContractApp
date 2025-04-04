using ContractApp.Infrastructure;
using ContractApp.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ContractApp.Views.Pages
{
    public partial class ContractSettingsPage : Page
    {
        public ContractSettingsPage()
        {
            InitializeComponent();
            Loaded += ContractSettingsPage_Loaded;
        }

        private async void ContractSettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        // Добавляем отсутствующий метод
        private async Task LoadDataAsync()
        {
            try
            {
                var data = await ContractSettingsRepository.GetAllAsync();
                SettingsGrid.ItemsSource = data
                    .OrderByDescending(s => s.ProxyDate);
                    
            }
            catch (Exception ex)
            {
                ShowNotification($"Ошибка загрузки: {ex.Message}", true);
            }
        }

        //private void SettingsGrid_Sorting(object sender, DataGridSortingEventArgs e)
        //{
        //    e.Handled = true; // Блокируем стандартную сортировку
        //}


        private async void AddSetting_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditContractSettingsWindow();
            if (window.ShowDialog() == true)
            {
                await LoadDataAsync();
                ShowNotification("Настройка успешно добавлена!", false);
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            var settings = (ContractSettings)((Button)sender).DataContext;
            var window = new EditContractSettingsWindow(settings);
            if (window.ShowDialog() == true)
            {
                await LoadDataAsync();
                ShowNotification("Настройка успешно обновлена!", false);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var settings = (ContractSettings)((Button)sender).DataContext;

            if (settings.IsActive)
            {
                ShowNotification("Нельзя удалить активную настройку!", true);
                return;
            }

            var message = $"Вы действительно хотите удалить настройку {settings.FullName}?";
            var dialog = new ConfirmationDialog(message);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() != true) return;

            try
            {
                await ContractSettingsRepository.DeleteAsync(settings.Id);
                await LoadDataAsync();
                ShowNotification("Настройка успешно удалена!", false);
            }
            catch (Exception ex)
            {
                ShowNotification($"Ошибка удаления: {ex.Message}", true);
            }
        }

        private void ShowNotification(string message, bool isError)
        {
            if (isError)
            {
                ErrorText.Text = message;
                SuccessText.Text = string.Empty;
            }
            else
            {
                SuccessText.Text = message;
                ErrorText.Text = string.Empty;
            }

            var timer = new DispatcherTimer { Interval = System.TimeSpan.FromSeconds(3) };
            timer.Tick += (s, args) =>
            {
                ErrorText.Text = string.Empty;
                SuccessText.Text = string.Empty;
                timer.Stop();
            };
            timer.Start();
        }

        private async void Activate_Click(object sender, RoutedEventArgs e)
        {
            var settings = (ContractSettings)((Button)sender).DataContext;
            try
            {
                var message = $"Активировать настройку для {settings.FullName}?";
                var dialog = new ConfirmationDialog(message);
                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() != true) return;

                await ContractSettingsRepository.ActivateAsync(settings.Id);
                await LoadDataAsync();
                ShowNotification("Настройка успешно активирована!", false);
            }
            catch (Exception ex)
            {
                ShowNotification($"Ошибка активации: {ex.Message}", true);
            }
        }
    }
}