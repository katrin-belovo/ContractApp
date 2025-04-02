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
            var textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text)) return;

            var originalText = textBox.Text;
            var caretIndex = textBox.CaretIndex;

            // Удаляем все точки и нецифровые символы
            var cleanText = new string(originalText.Where(char.IsDigit).ToArray());

            // Форматируем по шаблону XX.XX.XX
            var formattedText = new StringBuilder();
            for (int i = 0; i < cleanText.Length; i++)
            {
                if (i == 2 || i == 4) formattedText.Append('.');
                if (i >= 6) break; // Максимум 6 цифр
                formattedText.Append(cleanText[i]);
            }

            // Обновляем текст только если есть изменения
            if (originalText != formattedText.ToString())
            {
                textBox.Text = formattedText.ToString();

                // Корректируем позицию курсора
                var newCaretIndex = caretIndex + (formattedText.Length - originalText.Length);
                textBox.CaretIndex = Math.Min(newCaretIndex, formattedText.Length);
            }
        }

        private void CodeBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;

            // Разрешаем только цифры и управляющие клавиши
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9) &&
                !(e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) &&
                e.Key != Key.Back &&
                e.Key != Key.Delete &&
                e.Key != Key.Left &&
                e.Key != Key.Right)
            {
                e.Handled = true;
            }
        }
    }
}
