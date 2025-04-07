using System.Windows;
using System.Windows.Controls;
using ContractApp.Models;
using ContractApp.Infrastructure;
using System.Threading.Tasks;
using System.Windows.Data;
using System;

namespace ContractApp.Views.Pages
{
    public partial class ContractsListPage : Page
    {
        public ContractsListPage()
        {
            InitializeComponent();
            Loaded += ContractsListPage_Loaded;
        }

        private async void ContractsListPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadContractsAsync();
        }

        private async Task LoadContractsAsync()
        {
            ContractsGrid.ItemsSource = await ContractRepository.GetAllAsync();
        }

        private async void AddContract_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddContractWindow();
            if (window.ShowDialog() == true)
            {
                await LoadContractsAsync();
            }
        }

        private async void Conclude_Click(object sender, RoutedEventArgs e)
        {
            var contract = (Contract)((Button)sender).DataContext;
            await ContractRepository.ConcludeContractAsync(contract.Id);
            await LoadContractsAsync();
        }

        private async void Terminate_Click(object sender, RoutedEventArgs e)
        {
            var contract = (Contract)((Button)sender).DataContext;
            await ContractRepository.TerminateContractAsync(contract.Id);
            await LoadContractsAsync();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var contract = (Contract)((Button)sender).DataContext;
            // Реализация удаления договора
            await LoadContractsAsync();
        }
    }

    public class StatusToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}