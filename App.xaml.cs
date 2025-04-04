using ContractApp.Utilities;
using ContractApp.Views;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Markup;

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

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));

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
