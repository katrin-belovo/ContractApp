using ContractApp.Infrastructure;
using ContractApp.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ContractApp.Views
{
    public partial class EditTuitionFeeWindow : Window
    {
        private readonly TuitionFee _fee;

        public EditTuitionFeeWindow(TuitionFee fee = null)
        {
            InitializeComponent();
            _fee = fee ?? new TuitionFee();
            DataContext = _fee;
            LoadDirections();
            LoadYears();
        }

        private async void LoadDirections()
        {
            DirectionsCombo.ItemsSource = await DirectionRepository.GetAllAsync();
            if (_fee.DirectionId > 0)
            {
                DirectionsCombo.SelectedValue = _fee.DirectionId;
            }
        }

        private void LoadYears()
        {
            var currentYear = DateTime.Now.Year;
            YearCombo.ItemsSource = Enumerable.Range(currentYear - 5, 6).Reverse();
            if (_fee.Year == 0)
            {
                _fee.Year = currentYear;
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (_fee.DirectionId == 0)
                {
                    ErrorText.Text = "Выберите направление";
                    return;
                }

                if (_fee.Year < DateTime.Now.Year - 5 || _fee.Year > DateTime.Now.Year)
                {
                    ErrorText.Text = "Некорректный год набора";
                    return;
                }

                if (_fee.Amount <= 0)
                {
                    ErrorText.Text = "Введите корректную стоимость";
                    return;
                }

                await TuitionFeeRepository.AddAsync(_fee);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            var culture = CultureInfo.GetCultureInfo("ru-RU");

            // Заменяем точку на запятую во вводимом тексте
            string modifiedText = e.Text.Replace('.', ',');

            // Получаем текущий текст с учетом замены
            string newText = textBox.Text.Insert(textBox.CaretIndex, modifiedText);

            // Проверяем валидность нового текста
            bool isValid = decimal.TryParse(newText,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                culture,
                out decimal _);

            // Проверяем количество разделителей
            int separatorCount = modifiedText.Count(c => c == ',') +
                                textBox.Text.Count(c => c == ',');

            e.Handled = !isValid || separatorCount > 1;

            // Если не заблокировано, вручную добавляем измененный текст
            if (!e.Handled)
            {
                textBox.Text = newText;
                textBox.CaretIndex = textBox.Text.Length;
                e.Handled = true; // Блокируем стандартную обработку
            }
        }

        private void AmountTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var culture = CultureInfo.GetCultureInfo("ru-RU");

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "0,00";
                _fee.Amount = 0;
                return;
            }

            if (decimal.TryParse(textBox.Text, NumberStyles.Any, culture, out decimal amount))
            {
                _fee.Amount = Math.Round(amount, 2);
                textBox.Text = _fee.Amount.ToString("N2", culture) + " ₽";
            }
            else
            {
                textBox.Text = "0,00";
                _fee.Amount = 0;
            }
        }


        private int CountDecimalSeparators(string text)
{
    return text.Count(c => c == ',' || c == '.');
}

        private void AmountTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text == "0,00")
            {
                textBox.Text = string.Empty;
            }
        }
    }
}