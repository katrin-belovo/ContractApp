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
                    ErrorText.Text = "Нельзя удалить направление с привязанными группами!";
                    return;
                }

                await DirectionRepository.DeleteAsync(direction.Id);
                await LoadDataAsync();
                ErrorText.Text = "";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка удаления: {ex.Message}";
            }
        }
    }
}
