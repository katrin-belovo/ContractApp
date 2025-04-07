using System;
using System.Windows;
using System.Windows.Controls;
using ContractApp.Models;
using ContractApp.Infrastructure;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace ContractApp.Views
{
    public partial class AddContractWindow : Window
    {
        public Student FoundStudent { get; set; }
        public Representative FoundRepresentative { get; set; }

        public AddContractWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += AddContractWindow_Loaded;

            StudentEditControl.DataIsCorrect += () =>
            {
                FoundStudentBlock.Visibility = Visibility.Visible;
                StudentEditControl.Visibility = Visibility.Collapsed;
            };

            RepresentativeEditControl.DataIsCorrect += () =>
            {
                FoundRepresentativeBlock.Visibility = Visibility.Visible;
                RepresentativeEditControl.Visibility = Visibility.Collapsed;
            };

            // Инициализация ComboBox'ов без Items
            EducationLevelCombo.Items.Clear();
            DirectionCombo.Items.Clear();
        }

        private async void AddContractWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Загружаем направления
                var directions = await DirectionRepository.GetAllAsync();

                // Заполняем EducationLevelCombo через Items (не ItemsSource)
                EducationLevelCombo.Items.Add("СПО");
                EducationLevelCombo.Items.Add("ВО");
                EducationLevelCombo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка загрузки направлений: {ex.Message}";
            }
        }

        private void ContractTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RepresentativeGroup.Visibility = ContractTypeCombo.SelectedIndex == 1 ?
                Visibility.Visible : Visibility.Collapsed;

            OrganizationGroup.Visibility = ContractTypeCombo.SelectedIndex == 2 ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private async void SearchStudent_Click(object sender, RoutedEventArgs e)
        {
            string series = StudentPassportSeries.Text.Trim();
            string number = StudentPassportNumber.Text.Trim();

            if (string.IsNullOrWhiteSpace(series) || string.IsNullOrWhiteSpace(number))
            {
                ErrorText.Text = "Введите серию и номер паспорта студента";
                return;
            }

            try
            {
                FoundStudent = await StudentRepository.GetByPassportAsync(series, number);

                if (FoundStudent != null)
                {
                    FoundStudentBlock.Visibility = Visibility.Visible;
                    StudentEditControl.Visibility = Visibility.Collapsed;
                    //FoundStudentBlock.DataContext = this;
                    FoundStudentBlock.DataContext = FoundStudent;
                    DirectionGroup.Visibility = Visibility.Visible;
                    UpdateAvailableDirections();
                }
                else
                {
                    FoundStudentBlock.Visibility = Visibility.Collapsed;
                    StudentEditControl.Visibility = Visibility.Visible;
                    StudentEditControl.SetStudent(new Student
                    {
                        PassportSeries = series,
                        PassportNumber = number
                    }, isNewStudent: true);
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка поиска студента: {ex.Message}";
            }
        }

        private void UpdateAvailableDirections()
        {
            if (FoundStudent == null) return;

            // Очищаем и заполняем через Items
            EducationLevelCombo.Items.Clear();
            DirectionCombo.Items.Clear();

            if (FoundStudent.EducationBase.Contains("9 классов"))
            {
                EducationLevelCombo.Items.Add("СПО");
                EducationLevelCombo.SelectedIndex = 0;
                EducationLevelCombo.IsEnabled = false;
                LoadDirections("СПО");
            }
            else if (FoundStudent.EducationBase.Contains("11 классов"))
            {
                EducationLevelCombo.Items.Add("СПО");
                EducationLevelCombo.Items.Add("ВО");
                EducationLevelCombo.SelectedIndex = 0;
                EducationLevelCombo.IsEnabled = true;
                LoadDirections("СПО");
            }
            else
            {
                EducationLevelCombo.Items.Add("ВО");
                EducationLevelCombo.SelectedIndex = 0;
                EducationLevelCombo.IsEnabled = false;
                LoadDirections("ВО");
            }
        }

        private async void LoadDirections(string level)
        {
            try
            {
                DirectionCombo.Items.Clear();
                var directions = await DirectionRepository.GetAllAsync();
                var filteredDirections = directions.Where(d => d.Level == level).ToList();

                foreach (var direction in filteredDirections)
                {
                    DirectionCombo.Items.Add(direction);
                }

                if (DirectionCombo.Items.Count > 0)
                {
                    DirectionCombo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка загрузки направлений: {ex.Message}";
            }
        }

        private void EducationLevelCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EducationLevelCombo.SelectedItem == null) return;

            string level = EducationLevelCombo.SelectedItem.ToString();
            LoadDirections(level);
        }

        private async void SearchRepresentative_Click(object sender, RoutedEventArgs e)
        {
            string series = RepresentativePassportSeries.Text.Trim();
            string number = RepresentativePassportNumber.Text.Trim();

            if (string.IsNullOrWhiteSpace(series) || string.IsNullOrWhiteSpace(number))
            {
                ErrorText.Text = "Введите серию и номер паспорта представителя";
                return;
            }

            try
            {
                FoundRepresentative = await RepresentativeRepository.GetByPassportAsync(series, number);

                if (FoundRepresentative != null)
                {
                    FoundRepresentativeBlock.Visibility = Visibility.Visible;
                    RepresentativeEditControl.Visibility = Visibility.Collapsed;
                    FoundRepresentativeBlock.DataContext = FoundRepresentative;
                }
                else
                {
                    FoundRepresentativeBlock.Visibility = Visibility.Collapsed;
                    RepresentativeEditControl.Visibility = Visibility.Visible;
                    RepresentativeEditControl.SetRepresentative(new Representative
                    {
                        PassportSeries = series,
                        PassportNumber = number
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка поиска представителя: {ex.Message}";
            }
        }

        private void EditStudent_Click(object sender, RoutedEventArgs e)
        {
            FoundStudentBlock.Visibility = Visibility.Collapsed;
            StudentEditControl.Visibility = Visibility.Visible;
            StudentEditControl.SetStudent(FoundStudent, isNewStudent: false);
        }

        private void EditRepresentative_Click(object sender, RoutedEventArgs e)
        {
            FoundRepresentativeBlock.Visibility = Visibility.Collapsed;
            RepresentativeEditControl.Visibility = Visibility.Visible;
            RepresentativeEditControl.SetRepresentative(FoundRepresentative);
        }

        private void StudentEditControl_StudentSaved(Student student)
        {
            FoundStudent = student;
            FoundStudentBlock.Visibility = Visibility.Visible;
            StudentEditControl.Visibility = Visibility.Collapsed;
            FoundStudentBlock.DataContext = FoundStudent;
            UpdateAvailableDirections();
        }

        private void RepresentativeEditControl_RepresentativeSaved(Representative representative)
        {
            FoundRepresentative = representative;
            FoundRepresentativeBlock.Visibility = Visibility.Visible;
            RepresentativeEditControl.Visibility = Visibility.Collapsed;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (FoundStudent == null)
            {
                ErrorText.Text = "Необходимо ввести данные студента";
                return;
            }

            if (DirectionCombo.SelectedItem == null)
            {
                ErrorText.Text = "Необходимо выбрать направление подготовки";
                return;
            }

            try
            {
                var direction = (Direction)DirectionCombo.SelectedItem;
                var contract = new Contract
                {
                    Number = $"{direction.ShortName}-{DateTime.Now:yyyyMMdd-HHmmss}",
                    Status = "Черновик",
                    CreationDate = DateTime.Now
                };

                contract.Id = await ContractRepository.CreateBaseContractAsync(contract);

                switch (ContractTypeCombo.SelectedIndex)
                {
                    case 0: // Двусторонний
                        var bilateral = new BilateralContract
                        {
                            Id = contract.Id,
                            StudentId = FoundStudent.Id
                        };
                        await BilateralContractRepository.AddAsync(bilateral);
                        break;

                    case 1: // Трехсторонний
                        if (FoundRepresentative == null)
                        {
                            ErrorText.Text = "Для трехстороннего договора необходимо указать представителя";
                            return;
                        }

                        var trilateral = new TrilateralContract
                        {
                            Id = contract.Id,
                            StudentId = FoundStudent.Id,
                            RepresentativeId = FoundRepresentative.Id
                        };
                        await TrilateralContractRepository.AddAsync(trilateral);
                        break;

                    case 2: // С организацией
                        var orgContract = new OrganizationContract
                        {
                            Id = contract.Id,
                            StudentId = FoundStudent.Id,
                            OrganizationId = 1 // Временное значение
                        };
                        await OrganizationContractRepository.AddAsync(orgContract);
                        break;
                }

                MessageBox.Show("Договор успешно сохранен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Ошибка при сохранении: {ex.Message}";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}