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
using ContractApp.Models;
using ContractApp.ViewModels;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ContractApp.Views
{
    /// <summary>
    /// Логика взаимодействия для EditDirectionWindow.xaml
    /// </summary>
    public partial class EditDirectionWindow : Window
    {
        public EditDirectionWindow(Direction direction = null)
        {
            InitializeComponent();
            DataContext = new EditDirectionViewModel(direction ?? new Direction());
        }

        private void CodeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void CodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = CodeBox.Text.Replace(".", "");
            if (text.Length >= 2) text = text.Insert(2, ".");
            if (text.Length >= 5) text = text.Insert(5, ".");
            if (CodeBox.Text != text)
            {
                CodeBox.Text = text;
                CodeBox.CaretIndex = text.Length;
            }
        }
    }
}
