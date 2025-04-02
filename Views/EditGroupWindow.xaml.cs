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
using System.Windows.Shapes;

namespace ContractApp.Views
{
    /// <summary>
    /// Логика взаимодействия для EditGroupWindow.xaml
    /// </summary>
    public partial class EditGroupWindow : Window
    {
        private readonly Group _group;

        public EditGroupWindow(Group group = null)
        {
            InitializeComponent();
            _group = group ?? new Group();
            DataContext = _group;
            LoadDirections();
        }

        private async void LoadDirections()
        {
            DirectionsCombo.ItemsSource = await DirectionRepository.GetAllAsync();
            if (_group.DirectionId > 0)
            {
                DirectionsCombo.SelectedValue = _group.DirectionId;
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_group.Name))
                {
                    ErrorText.Text = "Введите название группы";
                    return;
                }

                if (_group.DirectionId == 0)
                {
                    ErrorText.Text = "Выберите направление";
                    return;
                }

                if (_group.Id == 0)
                    await GroupRepository.AddAsync(_group);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка: {ex.Message}";
            }
        }
    }
}
