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
        private void LoadHeadOfHouseholdComboBox()
        {
            var users = UserDAO.GetAllUsers();

            // Tạo danh sách mới để thêm "All"
            var userList = new List<User>();

            // Thêm lựa chọn "All" với UserId = -1
            userList.Add(new User { UserId = -1, FullName = "All" });

            // Thêm toàn bộ người dùng còn lại
            userList.AddRange(users);
            HeadOfHouseholdComboBox.ItemsSource = UserDAO.GetAllUsers();
            HeadOfHouseholdComboBox.DisplayMemberPath = "FullName";
            HeadOfHouseholdComboBox.SelectedValuePath = "UserId";
            HeadOfHouseholdSearchComboBox.ItemsSource = UserDAO.GetAllUsers();
            HeadOfHouseholdSearchComboBox.DisplayMemberPath = "FullName";
            HeadOfHouseholdSearchComboBox.SelectedValuePath = "UserId";
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (HeadOfHouseholdComboBox.SelectedValue.ToString().IsNullOrEmpty() || AddressTextBox.Text.IsNullOrEmpty() || CreatedDatePicker.SelectedDate.ToString().IsNullOrEmpty())
            {
                MessageBox.Show("Please fill in all fields!");
            }
            else
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
                MessageBox.Show("Household added successfully!");
                LoadHouseholds();
                ClearInputs();
            }
        }

        public void LoadInitialData2()
        {
            var household = HouseholdDAO.GetAllHouseholds();
            if (HeadOfHouseholdSearchComboBox.SelectedItem != null)
            {
                if (HeadOfHouseholdSearchComboBox.SelectedValue != null)
                {
                    int selectedUserId = (int)HeadOfHouseholdSearchComboBox.SelectedValue;
                    if (selectedUserId>0)
                    {
                        household = HouseholdDAO.GetHouseholdByName(selectedUserId, household);
                    }
                    else
                    {
                        household = HouseholdDAO.GetAllHouseholds();
                    }
                }
            }

            // Tìm kiếm theo ngày từ DatePicker
            if (DateSearchPicker.SelectedDate.HasValue)
            {
                DateOnly selectedDate = DateOnly.FromDateTime(DateSearchPicker.SelectedDate.Value);
                household = HouseholdDAO.GetUserByDate(selectedDate, household);
            }

            if (DateSearchPicker != null && !DateSearchPicker.Text.IsNullOrEmpty())
            {
                DateOnly address = DateOnly.Parse(DateSearchPicker.Text);
                household = HouseholdDAO.GetUserByDate(address, household);
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
                selectedHousehold.HeadOfHouseholdId = (int?)HeadOfHouseholdComboBox.SelectedValue;
                selectedHousehold.Address = AddressTextBox.Text;
                selectedHousehold.CreatedDate = CreatedDatePicker.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(CreatedDatePicker.SelectedDate.Value)
                    : DateOnly.FromDateTime(DateTime.Now);
                HouseholdDAO.UpdateHousehold(selectedHousehold);
                LoadHouseholds();
                ClearInputs();
            }
            else
            {
                MessageBox.Show("Please select a household to edit!");
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
                MessageBox.Show("Please select a household to delete!");
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
            LoadInitialData2();
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
            HeadOfHouseholdComboBox.SelectedIndex = -1;
            AddressTextBox.Text = "";
            CreatedDatePicker.SelectedDate = null;
        }
    }
}
