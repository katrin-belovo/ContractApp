using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ContractApp.Views.Pages;
using MaterialDesignThemes.Wpf;


namespace ContractApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button _activeButton;

        public string ActiveButtonTag
        {
            get { return (string)GetValue(ActiveButtonTagProperty); }
            set { SetValue(ActiveButtonTagProperty, value); }
        }


        public static readonly DependencyProperty ActiveButtonTagProperty =
            DependencyProperty.Register("ActiveButtonTag", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateActiveButton(e.Content);
        }

        private void UpdateActiveButton(object pageContent)
        {

            if (_activeButton != null)
                _activeButton.Style = (Style)FindResource("MainMenuButton");

            switch (pageContent)
            {
                case ContractsListPage _:
                case ContractSettingsPage _:
                    SetActiveButton(ContractsBtn);
                    break;
                case DirectionsPage _:
                case GroupsPage _:
                    SetActiveButton(DirectoriesBtn);
                    break;
                case ContingentPage _:
                    SetActiveButton(ContingentBtn);
                    break;
            }
        }

        private void SetActiveButton(Button button)
        {


            if (_activeButton != null)
            {
                _activeButton.ClearValue(Button.StyleProperty); // Сбрасываем явно
                _activeButton.Style = (Style)FindResource("MainMenuButton");
            }

            _activeButton = button;
            _activeButton.Style = (Style)FindResource("ActiveMenuButton");

        }

        private void ToggleContractsMenu(object sender, RoutedEventArgs e)
        {
            ContractsPopup.IsOpen = !ContractsPopup.IsOpen;
            DirectoriesPopup.IsOpen = false;
        }

        private void ToggleDirectoriesMenu(object sender, RoutedEventArgs e)
        {
            DirectoriesPopup.IsOpen = !DirectoriesPopup.IsOpen;
            ContractsPopup.IsOpen = false;
        }

        // Обработчики навигации
        private void ShowContractsList(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ContractsListPage());
            ContractsPopup.IsOpen = false;
            SetActiveButton(ContractsBtn);
        }

        private void ShowContractSettings(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ContractSettingsPage());
            ContractsPopup.IsOpen = false;
            SetActiveButton(ContractsBtn);
        }

        private void ShowDirections(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DirectionsPage());
            DirectoriesPopup.IsOpen = false;
            SetActiveButton(DirectoriesBtn);
        }

        private void ShowGroups(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GroupsPage());
            DirectoriesPopup.IsOpen = false;
            SetActiveButton(DirectoriesBtn);
        }

        private void ShowContingent(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ContingentPage());
            SetActiveButton((Button)sender);
            SetActiveButton(ContingentBtn);
        }


        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                ((Button)sender).Content = new PackIcon { Kind = PackIconKind.WindowMaximize };
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                ((Button)sender).Content = new PackIcon { Kind = PackIconKind.WindowRestore };
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Перемещение окна при захвате заголовка
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2) // Двойной клик для максимизации
                {
                    MaximizeWindow(null, null);
                }
                else
                {
                    this.DragMove();
                }
            }
        }
    }

}