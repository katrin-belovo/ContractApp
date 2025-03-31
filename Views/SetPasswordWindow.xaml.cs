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
    /// Логика взаимодействия для SetPasswordWindow.xaml
    /// </summary>
    public partial class SetPasswordWindow : Window
    {
        public SetPasswordWindow()
        {
            InitializeComponent();
        }

        private void SetPassword_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            if (password.Length < 6)
            {
                ErrorMessage.Text = "Пароль должен содержать не менее 6 символов";
                return;
            }

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                ErrorMessage.Text = "Пароль должен содуржать хотя бы один специальный символ";
                return;
            }

            if (password != confirm)
            {
                ErrorMessage.Text = "Вы неверно повторили пароль! Повторите попытку";
                return;
            }

            RegistryHelper.SavePassword(password);
            new LoginWindow().Show();
            this.Close();
        }
    }
}
