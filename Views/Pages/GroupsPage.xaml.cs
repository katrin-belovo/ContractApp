using ContractApp.Infrastructure;
using ContractApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// Логика взаимодействия для GroupsPage.xaml
    /// </summary>
    public partial class GroupsPage : Page
    {
        public GroupsPage()
        {
            InitializeComponent();
            Loaded += async (s, e) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            GroupsGrid.ItemsSource = await GroupRepository.GetAllAsync();
        }

        private async void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditGroupWindow();
            if (window.ShowDialog() == true)
            {
                await LoadDataAsync();
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var group = (Group)((Button)sender).DataContext;
            try
            {
                var message = $"Вы действительно хотите удалить группу {group.Name}?";
                var dialog = new ConfirmationDialog(message);
                dialog.Owner = Window.GetWindow(this);

                if (dialog.ShowDialog() != true) return;

                await GroupRepository.DeleteAsync(group.Id);
                await LoadDataAsync();

                ShowNotification($"Группа {group.Name} успешно удалена!", isError: false);
            }
            catch (Exception ex)
            {
                ShowNotification($"Ошибка удаления: {ex.Message}", isError: true);
                // Обработка ошибок
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
