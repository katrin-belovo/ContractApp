using System;
using System.Windows;
using System.Windows.Controls;
using ContractApp.Models;
using ContractApp.Infrastructure;
using System.Threading.Tasks;

namespace ContractApp.Views.Pages
{
    public partial class StudentEditControl : UserControl
    {
        public event Action<Student> StudentSaved;
        public event Action DataIsCorrect;
        private Student _student;
        private bool _isNewStudent;

        public StudentEditControl()
        {
            InitializeComponent();
        }

        public void SetStudent(Student student, bool isNewStudent)
        {
            _student = student;
            _isNewStudent = isNewStudent;

            LastNameTextBox.Text = student.LastName;
            FirstNameTextBox.Text = student.FirstName;
            MiddleNameTextBox.Text = student.MiddleName;
            PassportSeriesTextBox.Text = student.PassportSeries;
            PassportNumberTextBox.Text = student.PassportNumber;
            BirthDatePicker.SelectedDate = student.BirthDate;
            SnilsTextBox.Text = student.Snils;
            InnTextBox.Text = student.Inn;
            PhoneTextBox.Text = student.Phone;
            AddressTextBox.Text = student.Address;

            if (!string.IsNullOrEmpty(student.EducationBase))
            {
                foreach (ComboBoxItem item in EducationBaseCombo.Items)
                {
                    if (item.Content.ToString() == student.EducationBase)
                    {
                        EducationBaseCombo.SelectedItem = item;
                        break;
                    }
                }
            }

            SaveNewBtn.Visibility = _isNewStudent ? Visibility.Visible : Visibility.Collapsed;
            SaveChangesBtn.Visibility = !_isNewStudent ? Visibility.Visible : Visibility.Collapsed;
            DataIsCorrectBtn.Visibility = !_isNewStudent ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                BirthDatePicker.SelectedDate == null)
            {
                ErrorText.Text = "Заполните обязательные поля: Фамилия, Имя, Дата рождения";
                return false;
            }

            _student.LastName = LastNameTextBox.Text;
            _student.FirstName = FirstNameTextBox.Text;
            _student.MiddleName = MiddleNameTextBox.Text;
            _student.PassportSeries = PassportSeriesTextBox.Text;
            _student.PassportNumber = PassportNumberTextBox.Text;
            _student.BirthDate = BirthDatePicker.SelectedDate.Value;
            _student.Snils = SnilsTextBox.Text;
            _student.Inn = InnTextBox.Text;
            _student.Phone = PhoneTextBox.Text;
            _student.Address = AddressTextBox.Text;
            _student.EducationBase = (EducationBaseCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

            return true;
        }

        private async void SaveNew_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                _student.Id = await StudentRepository.AddAsync(_student);
                StudentSaved?.Invoke(_student);
                DataIsCorrect?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка при сохранении: {ex.Message}";
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                await StudentRepository.UpdateAsync(_student);
                StudentSaved?.Invoke(_student);
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка при обновлении: {ex.Message}";
            }
        }

        private void DataIsCorrect_Click(object sender, RoutedEventArgs e)
        {
            DataIsCorrect?.Invoke();
            StudentSaved?.Invoke(_student);
        }
    }
}