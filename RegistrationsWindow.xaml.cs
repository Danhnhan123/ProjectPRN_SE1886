using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProjectPRN_SE1886.Models;
using ProjectPRN_SE1886.viewModel;

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
            // Hiển thị thông tin người dùng hiện tại
            CurrentUserTextBlock.Text = $"{_currentUser.FullName} ({_currentUser.Email})";

            // Thiết lập ComboBox cho địa chỉ hộ gia đình
            HouseholdAddressComboBox.IsEditable = true;
            HouseholdAddressComboBox.IsTextSearchEnabled = false;
            var availableHouseholds = _allHouseholds.Where(h => !IsUserInHousehold(h.HouseholdId)).ToList();
            HouseholdAddressComboBox.ItemsSource = availableHouseholds;
            HouseholdAddressComboBox.SelectedValuePath = "HouseholdId";
            HouseholdAddressComboBox.DisplayMemberPath = "Address";

            // Thiết lập RegistrationType ComboBox
            RegistrationTypeComboBox.ItemsSource = new List<string> { "Permanent", "Temporary", "TemporaryStay" };
            RegistrationTypeComboBox.SelectedIndex = 0;

            // Thiết lập DatePicker
            StartDatePicker.SelectedDate = DateTime.Today.AddDays(1);
            StartDatePicker.BlackoutDates.AddDatesInPast();

            

            // Thiết lập ràng buộc cho EndDate
            EndDatePicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, StartDatePicker.SelectedDate.Value.AddDays(9)));

            // Thiết lập DataGrid lịch sử đăng ký
            RegistrationHistoryDataGrid.ItemsSource = _userRegistrations.Select(r => new
            {
                HouseholdAddress = _allHouseholds.FirstOrDefault(h => h.HouseholdId == r.HouseholdId)?.Address ?? "N/A",
                r.RegistrationType,
                r.StartDate,
                r.EndDate,
                r.Status,
                r.Comments
            });
        }

        // Kiểm tra xem user có trong hộ gia đình không
        private bool IsUserInHousehold(int householdId)
        {
            var member = _registrationsDAO.GetHouseholdMemberByUserId(_currentUser.UserId);
            return member != null && member.HouseholdId == householdId;
        }

        // Xử lý sự kiện khi text trong ComboBox thay đổi
        private void HouseholdAddressComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = HouseholdAddressComboBox.Text.ToLower();
            var availableHouseholds = _allHouseholds.Where(h => !IsUserInHousehold(h.HouseholdId)).ToList();
            if (string.IsNullOrEmpty(filter))
            {
                HouseholdAddressComboBox.ItemsSource = availableHouseholds;
            }
            else
            {
                HouseholdAddressComboBox.ItemsSource = availableHouseholds
                    .Where(h => h.Address.ToLower().Contains(filter))
                    .ToList();
            }
            HouseholdAddressComboBox.IsDropDownOpen = true;
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDatePicker.SelectedDate.HasValue)
            {
                EndDatePicker.BlackoutDates.Clear();
                EndDatePicker.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, StartDatePicker.SelectedDate.Value.AddDays(9)));
            }
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
                LoadData(); // Cập nhật lại dữ liệu
                SetupUI(); // Cập nhật lại giao diện
            }
        }

        private bool ValidateInput()
        {
            if (!StartDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a Start Date.");
                return false;
            }
            if (!EndDatePicker.SelectedDate.HasValue && RegistrationTypeComboBox.SelectedItem.ToString() != "Permanent")
            {
                MessageBox.Show("Please select an End Date.");
                return false;
            }
            if (HouseholdAddressComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a Household Address.");
                return false;
            }
            return true;
        }
    }
}