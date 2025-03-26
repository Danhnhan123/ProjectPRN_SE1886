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

namespace ProjectPRN_SE1886
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        private string _userRole;
        private User _currentUser;

        public DashboardWindow(User user)
        {
            InitializeComponent();
            _userRole = user.Role;
            ConfigureRoleAccess();
            _currentUser = user;
        }

        private void ConfigureRoleAccess()
        {
            switch (_userRole)
            {
                case "Citizen":
                    UsersButton.IsEnabled = false;
                    HouseholdsButton.IsEnabled = false;
                    MembersButton.IsEnabled = false;
                    LogsButton.IsEnabled = false;
                    break;
                case "AreaLeader":
                    UsersButton.IsEnabled = false;
                    LogsButton.IsEnabled = false;
                    ProfileButton.Visibility = Visibility.Hidden;
                    break;
                case "Police":
                    UsersButton.IsEnabled = false;
                    ProfileButton.Visibility = Visibility.Hidden;
                    break;
                case "Administrator":
                    HouseholdsButton.IsEnabled = false;
                    MembersButton.IsEnabled = false;
                    LogsButton.IsEnabled = false;
                    NotificationsButton.IsEnabled = false;
                    RegistrationsButton.IsEnabled = false;
                    ProfileButton.Visibility = Visibility.Hidden;
                    // All buttons enabled
                    break;
            }
        }

        private void HouseholdsButton_Click(object sender, RoutedEventArgs e)
        {
            HouseholdManagementWindow householdManagementWindow = new HouseholdManagementWindow(_userRole);
            householdManagementWindow.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            UserManagementWindow window = new UserManagementWindow();
            window.Show();
        }

        private void RegistrationsManageButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationsManageWindow window = new RegistrationsManageWindow(_user);
            window.Show();
        }
        private void RegistrationsButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationsWindow wndow = new RegistrationsWindow(_user);
            wndow.Show();
        }

        private void MembersButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NotificationsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LogsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfileWindow window = new UserProfileWindow(_currentUser);
            window.Show();
        }
    }
}
