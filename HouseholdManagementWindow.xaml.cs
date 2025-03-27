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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.IdentityModel.Tokens;
using ProjectPRN_SE1886.Models;
using static ProjectPRN_SE1886.DAOs;

namespace ProjectPRN_SE1886
{
    /// <summary>
    /// Interaction logic for HouseholdManagementWindow.xaml
    /// </summary>
    public partial class HouseholdManagementWindow : Window
    {
        private readonly HouseholdDAO _householdDAO;
        private readonly string _userRole;
        public HouseholdManagementWindow(string userRole)
        {
            InitializeComponent();
            _householdDAO = new HouseholdDAO();
            _userRole = userRole;
            LoadHouseholds();
            ConfigureRoleAccess();
        }
        private void ConfigureRoleAccess()
        {
            switch (_userRole)
            {
                case "Citizen":
                    AddButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;
                case "Area Leader":
                    AddButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;
                    // Police và Admin có toàn quyền
            }
        }

        private void LoadHouseholds()
        {
            HouseholdsDataGrid.ItemsSource = HouseholdDAO.GetAllHouseholds();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string h = HeadOfHouseholdComboBox.Text;
            string n = HouseholdNumberTextBox.Text;

            var user = DAO.UserDAO.GetUserByCccd(h);
            if (user == null)
            {
                return;
            }
            if (HouseholdDAO.IsHeadOfHouseholdExists(user.UserId))
            {
                MessageBox.Show("Head of household is already exists! Please enter another head of household.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (HouseholdDAO.IsHouseholdNumberExists(n))
            {
                MessageBox.Show("Household number is already exists! Please enter another household number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (HeadOfHouseholdComboBox.Text.IsNullOrEmpty() || AddressTextBox.Text.IsNullOrEmpty() || CreatedDatePicker.SelectedDate.ToString().IsNullOrEmpty() || HouseholdNumberTextBox.Text.IsNullOrEmpty())
            {
                MessageBox.Show("Please fill in all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Household household = new Household
                {
                    HeadOfHouseholdId = user.UserId,
                    Address = AddressTextBox.Text,
                    HouseholdNumber = HouseholdNumberTextBox.Text,
                    CreatedDate = CreatedDatePicker.SelectedDate.HasValue
                   ? DateOnly.FromDateTime(CreatedDatePicker.SelectedDate.Value)
                   : DateOnly.FromDateTime(DateTime.Now)
                };
                HouseholdDAO.AddHousehold(household);
                MessageBox.Show("Household added successfully!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadHouseholds();
                ClearInputs();
            }
        }

        public void LoadInitialData2()
        {
            var household = HouseholdDAO.GetAllHouseholds();
            if (HeadOfHouseholdSearchComboBox != null && !HeadOfHouseholdSearchComboBox.Text.IsNullOrEmpty())
            {
                string headOfHousehold = HeadOfHouseholdSearchComboBox.Text;
                household = HouseholdDAO.GetUserByHeadOfHousehold(headOfHousehold, household);
            }

            // Tìm kiếm theo ngày từ DatePicker
            if (DateSearchPicker.SelectedDate.HasValue)
            {
                DateOnly selectedDate = DateOnly.FromDateTime(DateSearchPicker.SelectedDate.Value);
                household = HouseholdDAO.GetUserByDate(selectedDate, household);
            }

            if (AddressSearchTextBox != null && !AddressSearchTextBox.Text.IsNullOrEmpty())
            {
                string address = AddressSearchTextBox.Text;
                household = HouseholdDAO.GetUserByAddress(address, household);
            }
            HouseholdsDataGrid.ItemsSource = household;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (HouseholdsDataGrid.SelectedItem is Household selectedHousehold)
            {
                selectedHousehold.HeadOfHouseholdId = DAO.UserDAO.GetUserByCccd(HeadOfHouseholdComboBox.Text).UserId;
                selectedHousehold.Address = AddressTextBox.Text;
                selectedHousehold.HouseholdNumber = HouseholdNumberTextBox.Text;
                selectedHousehold.CreatedDate = CreatedDatePicker.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(CreatedDatePicker.SelectedDate.Value)
                    : DateOnly.FromDateTime(DateTime.Now);
                HouseholdDAO.UpdateHousehold(selectedHousehold);
                MessageBox.Show("Household updated successfully!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadHouseholds();
                ClearInputs();
            }
            else
            {
                MessageBox.Show("Please select a household to edit!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (HouseholdsDataGrid.SelectedItem is Household selectedHousehold)
            {
                _householdDAO.DeleteHousehold(selectedHousehold.HouseholdId);
                LoadHouseholds();
                ClearInputs();
            }
            else
            {
                MessageBox.Show("Please select a household to delete!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HouseholdsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HouseholdsDataGrid.SelectedItem is Household selectedHousehold)
            {
                HouseholdIdTextBox.Text = selectedHousehold.HouseholdId.ToString();
                HeadOfHouseholdComboBox.Text = selectedHousehold.HeadOfHousehold.Cccd;
                AddressTextBox.Text = selectedHousehold.Address;
                HouseholdNumberTextBox.Text = selectedHousehold.HouseholdNumber;
                // Chuyển DateOnly sang DateTime để hiển thị trong DatePicker
                CreatedDatePicker.SelectedDate = selectedHousehold.CreatedDate.HasValue
                    ? selectedHousehold.CreatedDate.Value.ToDateTime(TimeOnly.MinValue)
                    : null;
            }
        }



        private void create_change(object sender, SelectionChangedEventArgs e)
        {
            LoadInitialData2();
        }

        private void address_keydown(object sender, KeyEventArgs e)
        {
            LoadInitialData2();
        }

        private void ClearInputs()
        {
            HouseholdIdTextBox.Text = "";
            HeadOfHouseholdComboBox.Text = "";
            AddressTextBox.Text = "";
            HouseholdNumberTextBox.Text = "";
            CreatedDatePicker.SelectedDate = null;
        }

        private void head_selection(object sender, KeyEventArgs e)
        {
            LoadInitialData2();
        }
    }
}
