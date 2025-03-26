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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.IdentityModel.Tokens;
using ProjectPRN_SE1886.Models;
using static ProjectPRN_SE1886.DAO;

namespace ProjectPRN_SE1886
{
    /// <summary>
    /// Interaction logic for HouseholdManagementPage.xaml
    /// </summary>
    public partial class HouseholdManagementPage : Page
    {

        private readonly HouseholdDAO _householdDAO;
        private readonly string _userRole;

        public HouseholdManagementPage(string userRole)
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
            HouseholdsDataGrid.ItemsSource = HouseholdDAO.GetAllHouseholds();
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
            HouseholdDAO.AddHousehold(household);
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
                HouseholdDAO.UpdateHousehold(selectedHousehold);
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
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Khôi phục danh sách đầy đủ từ database
            LoadHouseholds();
            // Xóa các điều kiện tìm kiếm
            HeadOfHouseholdSearchComboBox.SelectedIndex = -1;
            AddressSearchTextBox.Text = "";
            DateSearchPicker.SelectedDate = null;
        }

        

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement search logic here using DAO
            // Ví dụ:
            var households = HouseholdDAO.GetAllHouseholds();
            if (HeadOfHouseholdSearchComboBox.SelectedValue != null)
            {
                households = households.Where(h => h.HeadOfHouseholdId == (int?)HeadOfHouseholdSearchComboBox.SelectedValue).ToList();
            }
            if (!string.IsNullOrEmpty(AddressSearchTextBox.Text))
            {
                households = households.Where(h => h.Address.Contains(AddressSearchTextBox.Text)).ToList();
            }
            if (DateSearchPicker.SelectedDate.HasValue)
            {
                DateOnly searchDate = DateOnly.FromDateTime(DateSearchPicker.SelectedDate.Value);
                households = households.Where(h => h.CreatedDate == searchDate).ToList();
            }
            HouseholdsDataGrid.ItemsSource = households;
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

