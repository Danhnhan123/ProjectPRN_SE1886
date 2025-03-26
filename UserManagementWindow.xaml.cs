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
using static ProjectPRN_SE1886.DAO;

namespace ProjectPRN_SE1886
{
    /// <summary>
    /// Interaction logic for UserManagementWindow.xaml
    /// </summary>
    public partial class UserManagementWindow : Window
    {
        private readonly string _userRole;
        public UserManagementWindow()
        {
            InitializeComponent();
            LoadUsers();
            LoadInitialData();
            LoadInitialData1();
            RoleComboBox.SelectedIndex = 0;
        }

        public void LoadUsers()
        {
            var users = UserDAO.GetAllUsers();
            UserDataGrid.ItemsSource = users;
        }

        public void LoadInitialData()
        {
            if (cbRole.SelectedItem == null)
            {
                cbRole.Items.Add("Administrator");
                cbRole.Items.Add("Police");
                cbRole.Items.Add("AreaLeader");
                cbRole.Items.Add("Citizen");
            }
        }
        public void LoadInitialData1()
        {
            if (RoleComboBox.SelectedItem == null)
            {
                RoleComboBox.Items.Add("All");
                RoleComboBox.Items.Add("Administrator");
                RoleComboBox.Items.Add("Police");
                RoleComboBox.Items.Add("AreaLeader");
                RoleComboBox.Items.Add("Citizen");
            }
        }

        public void LoadInitialData2()
        {
            var users = UserDAO.GetAllUsers();
            if (RoleComboBox.SelectedItem != null)
            {
                if (RoleComboBox.SelectedItem.ToString().Equals("All"))
                {

                    UserDataGrid.ItemsSource = users;
                }
                else
                {
                    string role = RoleComboBox.SelectedItem.ToString();
                    users = UserDAO.GetUserByRole(role, users);
                    UserDataGrid.ItemsSource = users;
                }
            }
            if (FullnameSearchTextBox != null && !FullnameSearchTextBox.Text.IsNullOrEmpty())
            {
                string address = FullnameSearchTextBox.Text;
                users = UserDAO.GetUserByName(address, users);
                UserDataGrid.ItemsSource = users;
            }
            if (AddressSearchTextBox != null && !AddressSearchTextBox.Text.IsNullOrEmpty())
            {
                string address = AddressSearchTextBox.Text;
                users = UserDAO.GetUserByEmail(address, users);
                UserDataGrid.ItemsSource = users;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!FullnameSearchTextBox.Text.IsNullOrEmpty() && !EmailTextBox.Text.IsNullOrEmpty() && !PasswordTextBox.Text.IsNullOrEmpty() && !cbRole.SelectedItem.ToString().IsNullOrEmpty()) {
                User household = new User
                {
                    FullName = FullnameTextBox.Text,
                    Email = EmailTextBox.Text,
                    Password = PasswordTextBox.Text,
                    Role = cbRole.SelectedItem.ToString(),
                };
                UserDAO.AddUser(household);
                MessageBox.Show("User added successfully!");
                LoadInitialData2();
                ClearInputs();
            }
            else
            {
                MessageBox.Show("Please fill in all fields!");
            }

        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is User selectedHousehold)
            {
                selectedHousehold.FullName = FullnameTextBox.Text;
                selectedHousehold.Email = EmailTextBox.Text;
                selectedHousehold.Password = PasswordTextBox.Text;
                selectedHousehold.Role = cbRole.SelectedItem.ToString();
                UserDAO.UpdateUser(selectedHousehold);
                MessageBox.Show("User updated successfully!");
                LoadInitialData2();
                ClearInputs();
            }
            else
            {
                MessageBox.Show("Please select a user to edit!");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is User selectedHousehold)
            {
                UserDAO.DeleteUser(selectedHousehold.UserId);
                MessageBox.Show("User deleted successfully!");
                LoadInitialData2();
                ClearInputs();
            }
            else
            {
                MessageBox.Show("Please select a user to delete!");
            }
        }

        private void cbRole_selection(object sender, SelectionChangedEventArgs e)
        {
            LoadInitialData2();
        }

        private void UserDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is User selectedUser)
            {
                UserIdTextBox.Text = selectedUser.UserId.ToString();
                FullnameTextBox.Text = selectedUser.FullName;
                EmailTextBox.Text = selectedUser.Email;
                cbRole.SelectedItem = selectedUser.Role;
                PasswordTextBox.Text = selectedUser.Password;
            }
        }

        private void txtname_keydown(object sender, KeyEventArgs e)
        {
            LoadInitialData2();
        }

        private void txtAddress_keydown(object sender, KeyEventArgs e)
        {
            LoadInitialData2();
        }

        private void ClearInputs()
        {
            UserIdTextBox.Text = "";
            cbRole.SelectedIndex = -1;
            FullnameTextBox.Text = "";
            EmailTextBox.Text = "";
            PasswordTextBox.Text = "";
        }
    }
}
