using System;
using System.Windows;
using System.Windows.Controls;
using ContractApp.Models;
using ContractApp.Infrastructure;
using System.Threading.Tasks;

namespace ContractApp.Views.Pages
{
    public partial class RepresentativeEditControl : UserControl
    {
        public event Action<Representative> RepresentativeSaved;
        private Representative _representative;
        public event Action DataIsCorrect;

        public RepresentativeEditControl()
        {
            InitializeComponent();
        }

        private void DataIsCorrect_Click(object sender, RoutedEventArgs e)
        {
            DataIsCorrect?.Invoke();
        }

        public void SetRepresentative(Representative representative)
        {
            _representative = representative;

            LastNameTextBox.Text = representative.LastName;
            FirstNameTextBox.Text = representative.FirstName;
            MiddleNameTextBox.Text = representative.MiddleName;
            PassportSeriesTextBox.Text = representative.PassportSeries;
            PassportNumberTextBox.Text = representative.PassportNumber;
            SnilsTextBox.Text = representative.Snils;
            InnTextBox.Text = representative.Inn;
            PhoneTextBox.Text = representative.Phone;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PassportSeriesTextBox.Text) ||
                string.IsNullOrWhiteSpace(PassportNumberTextBox.Text))
            {
                ErrorText.Text = "Заполните обязательные поля: Фамилия, Имя, Паспортные данные";
                return;
            }

            try
            {
                _representative.LastName = LastNameTextBox.Text;
                _representative.FirstName = FirstNameTextBox.Text;
                _representative.MiddleName = MiddleNameTextBox.Text;
                _representative.PassportSeries = PassportSeriesTextBox.Text;
                _representative.PassportNumber = PassportNumberTextBox.Text;
                _representative.Snils = SnilsTextBox.Text;
                _representative.Inn = InnTextBox.Text;
                _representative.Phone = PhoneTextBox.Text;

                if (_representative.Id == 0)
                {
                    _representative.Id = await RepresentativeRepository.AddAsync(_representative);
                }
                else
                {
                    await RepresentativeRepository.UpdateAsync(_representative);
                }

                RepresentativeSaved?.Invoke(_representative);
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка при сохранении: {ex.Message}";
            }
        }
    }
}