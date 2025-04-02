using ContractApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ContractApp.Models;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ContractApp.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для DirectionsPage.xaml
    /// </summary>
    public partial class DirectionsPage : Page
    {
        public DirectionsPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                DirectionsGrid.ItemsSource = await DirectionRepository.GetAllAsync();
                ErrorText.Text = "";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка загрузки данных: {ex.Message}";
            }
        }

        private async void AddDirection_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditDirectionWindow();
            if (window.ShowDialog() == true)
            {
                await LoadDataAsync();
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            var direction = (Direction)((Button)sender).DataContext;
            var window = new EditDirectionWindow(direction);
            if (window.ShowDialog() == true)
            {
                await LoadDataAsync();
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var direction = (Direction)((Button)sender).DataContext;

            try
            {
                if (await GroupRepository.HasGroupsAsync(direction.Id))
                {
                    var msg = "Нельзя удалить направление с привязанными группами!";
                    ShowNotification($"Ошибка удаления: {msg}", isError: true);
                    
                    return;
                }
                // Диалог подтверждения
                var message = $"Вы действительно хотите удалить направление {direction.Code} {direction.FullName}?";
                var dialog = new ConfirmationDialog(message);
                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() != true) return;


                await DirectionRepository.DeleteAsync(direction.Id);
                await LoadDataAsync();
                ErrorText.Text = "";

                // Уведомление об успехе
                ShowNotification($"Направление {direction.Code} успешно удалено!", isError: false);
            }
            catch (Exception ex)
            {
                ShowNotification($"Ошибка удаления: {ex.Message}", isError: true);
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

            // Автоочистка через 3 секунды
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            timer.Tick += (s, e) =>
            {
                ErrorText.Text = string.Empty;
                SuccessText.Text = string.Empty;
                timer.Stop();
            };
            timer.Start();
        }
    }
}
