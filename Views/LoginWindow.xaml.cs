using ContractApp.Infrastructure;
using ContractApp.Utilities;
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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // Ваша проверка авторизации...
                if (RegistryHelper.ValidatePassword(PasswordBox.Password))
                {
                    await DatabaseService.InitializeDatabaseAsync();
                    new MainWindow().Show();
                    this.Close();
                }
                else
                {
                    ErrorMessage.Text = "Неверный пароль! Повторите попытку";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = ex.Message;
            }
            
        }
    }
}
