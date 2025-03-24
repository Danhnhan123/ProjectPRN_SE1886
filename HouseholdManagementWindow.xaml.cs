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
using ProjectPRN_SE1886.Models;
using static ProjectPRN_SE1886.DAO;

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
            LoadHeadOfHouseholdComboBox();
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
                    DeleteButton.IsEnabled = false;
                    break;
                    // Police và Admin có toàn quyền
            }
        }

        private void LoadHouseholds()
        {
            HouseholdsDataGrid.ItemsSource = _householdDAO.GetAllHouseholds();
        }
        private void LoadHeadOfHouseholdComboBox()
        {
            HeadOfHouseholdComboBox.ItemsSource = UserDAO.GetAllUsers();
            HeadOfHouseholdComboBox.DisplayMemberPath = "FullName";
            HeadOfHouseholdComboBox.SelectedValuePath = "UserId";
            HeadOfHouseholdSearchComboBox.ItemsSource = UserDAO.GetAllUsers();
            HeadOfHouseholdSearchComboBox.DisplayMemberPath = "FullName";
            HeadOfHouseholdSearchComboBox.SelectedValuePath = "UserId";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Household household = new Household
            {
                HeadOfHouseholdId = (int?)HeadOfHouseholdComboBox.SelectedValue,
                Address = AddressTextBox.Text,
                CreatedDate = CreatedDatePicker.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(CreatedDatePicker.SelectedDate.Value)
                    : DateOnly.FromDateTime(DateTime.Now)
            };
            _householdDAO.AddHousehold(household);
            LoadHouseholds();
            ClearInputs();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (HouseholdsDataGrid.SelectedItem is Household selectedHousehold)
            {
                selectedHousehold.HeadOfHouseholdId = (int?)HeadOfHouseholdComboBox.SelectedValue;
                selectedHousehold.Address = AddressTextBox.Text;
                selectedHousehold.CreatedDate = CreatedDatePicker.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(CreatedDatePicker.SelectedDate.Value)
                    : DateOnly.FromDateTime(DateTime.Now);
                _householdDAO.UpdateHousehold(selectedHousehold);
                LoadHouseholds();
                ClearInputs();
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
        }

        private void HouseholdsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HouseholdsDataGrid.SelectedItem is Household selectedHousehold)
            {
                HouseholdIdTextBox.Text = selectedHousehold.HouseholdId.ToString();
                HeadOfHouseholdComboBox.SelectedValue = selectedHousehold.HeadOfHouseholdId;
                AddressTextBox.Text = selectedHousehold.Address;
                // Chuyển DateOnly sang DateTime để hiển thị trong DatePicker
                CreatedDatePicker.SelectedDate = selectedHousehold.CreatedDate.HasValue
                    ? selectedHousehold.CreatedDate.Value.ToDateTime(TimeOnly.MinValue)
                    : null;
            }
        }


        private void head_selection(object sender, SelectionChangedEventArgs e)
        {

        }

        private void create_change(object sender, SelectionChangedEventArgs e)
        {

        }

        private void address_keydown(object sender, KeyEventArgs e)
        {

        }

        private void ClearInputs()
        {
            HouseholdIdTextBox.Text = "";
            HeadOfHouseholdComboBox.SelectedIndex = -1;
            AddressTextBox.Text = "";
            CreatedDatePicker.SelectedDate = null;
        }
    }
}
