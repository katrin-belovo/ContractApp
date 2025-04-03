using ContractApp.Infrastructure;
using ContractApp.Models;
using ContractApp.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ContractApp.Views.Pages
{
    public partial class TuitionFeesPage : Page
    {
        public TuitionFeesPage()
        {
            InitializeComponent();
            Loaded += TuitionFeesPage_Loaded; // Подписываемся на событие загрузки
        }

        private async void TuitionFeesPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadData(); // Асинхронная загрузка данных при загрузке страницы
        }

        private async Task LoadData()
        {
            try
            {
                var fees = await TuitionFeeRepository.GetAllAsync();
                FeesGrid.ItemsSource = fees?.Any() == true ? fees : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private async void AddFee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new EditTuitionFeeWindow();
                if (dialog.ShowDialog() == true)
                {
                    await LoadData(); // Добавляем await
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}");
            }
        }

        private async void DeleteFee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FeesGrid.SelectedItem is TuitionFee fee)
                {
                    var message = $"Вы действительно хотите удалить стоимость обучения для направления {fee.Direction.FullName} {fee.Amount}?";
                    var dialog = new ConfirmationDialog(message);
                    dialog.Owner = Window.GetWindow(this);

                    if (dialog.ShowDialog() != true) return;
                    await TuitionFeeRepository.DeleteAsync(fee.Id);
                    await LoadData(); // Добавляем await

                    ShowNotification($"Стоимсть обучения для направления {fee.Direction.FullName} успешно удалена!", isError: false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}");
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