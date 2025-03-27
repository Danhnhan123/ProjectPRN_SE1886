using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProjectPRN_SE1886.Models;

namespace ProjectPRN_SE1886
{
    public partial class RegistrationsWindow : Window
    {
        private readonly RegistrationsDAO _registrationsDAO;
        private readonly User _currentUser;
        private List<Household> _allHouseholds;
        private List<Registration> _userRegistrations;

        public RegistrationsWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _registrationsDAO = new RegistrationsDAO();
            LoadData();
            SetupUI();
        }

        private void LoadData()
        {
            _allHouseholds = _registrationsDAO.GetHouseholds();
            _userRegistrations = _registrationsDAO.GetRegistrationsByUserId(_currentUser.UserId);
        }

        private void SetupUI()
        {
            CurrentUserTextBlock.Text = $"{_currentUser.FullName} ({_currentUser.Cccd})";

            HouseholdAddressComboBox.IsEditable = true;
            HouseholdAddressComboBox.IsTextSearchEnabled = false;
            var availableHouseholds = _allHouseholds.ToList(); 
            HouseholdAddressComboBox.ItemsSource = availableHouseholds;
            HouseholdAddressComboBox.SelectedValuePath = "HouseholdId";
            HouseholdAddressComboBox.DisplayMemberPath = "AddressDisplay"; 


            RegistrationTypeComboBox.ItemsSource = new List<string> { "Permanent", "Temporary", "TemporaryStay", "MoveOut" };
            RegistrationTypeComboBox.SelectedIndex = 0;
            RegistrationTypeComboBox.SelectionChanged += RegistrationTypeComboBox_SelectionChanged;

            StartDatePicker.SelectedDate = DateTime.Today.AddDays(1);
            StartDatePicker.BlackoutDates.AddDatesInPast();

            RegistrationHistoryDataGrid.ItemsSource = _userRegistrations.Select(r => new
            {
                HouseholdAddress = _allHouseholds.FirstOrDefault(h => h.HouseholdId == r.HouseholdId)?.AddressDisplay ?? "N/A",
                r.RegistrationType,
                r.StartDate,
                r.EndDate,
                r.Status,
                r.Comments
            });
        }

        private void RegistrationTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedType = RegistrationTypeComboBox.SelectedItem?.ToString();
            var selectedHousehold = HouseholdAddressComboBox.SelectedItem as Household;
            var member = _registrationsDAO.GetHouseholdMemberByUserId(_currentUser.UserId);

            if (StartDatePicker.SelectedDate.HasValue)
            {
                EndDatePicker.BlackoutDates.Clear();
                EndDatePicker.BlackoutDates.AddDatesInPast();
                switch (selectedType)
                {
                    case "MoveOut":
                        EndDatePicker.IsEnabled = false;
                        EndDatePicker.BlackoutDates.Clear();
                        EndDatePicker.SelectedDate = null;
                        break;
                    case "Permanent":
                        EndDatePicker.IsEnabled = false;
                        EndDatePicker.BlackoutDates.Clear();
                        EndDatePicker.SelectedDate = null;
                        break;
                    case "Temporary":
                        EndDatePicker.IsEnabled = true;
                        EndDatePicker.BlackoutDates.Clear();
                        EndDatePicker.SelectedDate = null; 
                        EndDatePicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, StartDatePicker.SelectedDate.Value.AddDays(29)));
                        break;
                    case "TemporaryStay":
                        EndDatePicker.IsEnabled = true;
                        EndDatePicker.BlackoutDates.Clear();
                        EndDatePicker.SelectedDate = null; 
                        EndDatePicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, StartDatePicker.SelectedDate.Value));
                        EndDatePicker.BlackoutDates.Add(new CalendarDateRange(StartDatePicker.SelectedDate.Value.AddDays(30), DateTime.MaxValue));
                        break;
                }
            }
        }

        private void HouseholdAddressComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = HouseholdAddressComboBox.Text.ToLower();
            var availableHouseholds = _allHouseholds.ToList();

            if (string.IsNullOrEmpty(filter))
            {
                HouseholdAddressComboBox.ItemsSource = availableHouseholds;
            }
            else
            {
                HouseholdAddressComboBox.ItemsSource = availableHouseholds
                    .Where(h => h.AddressDisplay.ToLower().Contains(filter))
                    .ToList();
            }
            HouseholdAddressComboBox.IsDropDownOpen = true;
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                var selectedHousehold = HouseholdAddressComboBox.SelectedItem as Household;
                var registration = new Registration
                {
                    UserId = _currentUser.UserId,
                    HouseholdId = selectedHousehold?.HouseholdId,
                    RegistrationType = RegistrationTypeComboBox.SelectedItem.ToString(),
                    StartDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value),
                    EndDate = EndDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value) : (DateOnly?)null,
                    Status = "Pending",
                    Comments = CommentsTextBox.Text
                };

                _registrationsDAO.AddRegistration(registration);
                MessageBox.Show("Registration submitted successfully!");
                LoadData();
                SetupUI();
            }
        }

        private bool ValidateInput()
        {
            if (HouseholdAddressComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a Household Address.");
                return false;
            }

            if (!StartDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a Start Date.");
                return false;
            }

            var selectedType = RegistrationTypeComboBox.SelectedItem?.ToString();
            if (selectedType == "Temporary" || selectedType == "TemporaryStay")
            {
                if (!EndDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Please select an End Date for Temporary or TemporaryStay registration.");
                    return false;
                }

                if (selectedType == "Temporary" && EndDatePicker.SelectedDate <= StartDatePicker.SelectedDate)
                {
                    MessageBox.Show("End Date must be greater than Start Date for Temporary registration.");
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(CommentsTextBox.Text))
            {
                MessageBox.Show("Please provide comments for the registration.");
                return false;
            }

            return true;
        }

        private void HouseholdAddressComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedHousehold = HouseholdAddressComboBox.SelectedItem as Household;
            var member = _registrationsDAO.GetHouseholdMemberByUserId(_currentUser.UserId);

            if (selectedHousehold != null && member != null && member.HouseholdId == selectedHousehold.HouseholdId)
            {
                RegistrationTypeComboBox.ItemsSource = new List<string> { "MoveOut" };
                RegistrationTypeComboBox.SelectedIndex = 0;
                RegistrationTypeComboBox.IsEnabled = false;
            }
            else
            {
                RegistrationTypeComboBox.ItemsSource = new List<string> { "Permanent", "Temporary", "TemporaryStay" };
                RegistrationTypeComboBox.IsEnabled = true;
                RegistrationTypeComboBox.SelectedIndex = 0;
            }
        }
    }
}