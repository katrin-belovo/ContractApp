using ContractApp.Utilities;
using ContractApp.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ContractApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!RegistryHelper.IsPasswordSet())
            {
                new SetPasswordWindow().Show();
            }
            else
            {
                new LoginWindow().Show();
            }
        }
    }

}
