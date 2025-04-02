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
    /// Логика взаимодействия для ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog : Window
    {
        public ConfirmationDialog(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
            this.Closing += (s, e) =>
            {
                // Если закрыли через крестик - считаем как "Нет"
                if (this.DialogResult == null)
                    this.DialogResult = false;
            };
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
