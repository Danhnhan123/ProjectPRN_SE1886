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

namespace ProjectPRN_SE1886
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        private string _userRole;
        private User _currentUser;
        private string? _currentCccd;
        private string? _currentAddress;

        public DashboardWindow(User user)
        {
            InitializeComponent();
            _userRole = user.Role;
            _currentCccd = user.Cccd;
            _currentAddress = user.Address;
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
                    LogsButton.IsEnabled = false;
                    RegistrationsManageButton.IsEnabled = false;
                    MemberManageButton.IsEnabled = false;
                    if (_currentCccd.IsNullOrEmpty() || _currentAddress.IsNullOrEmpty())
                    {
                        RegistrationsButton.IsEnabled = false;
                        MessageBox.Show("You must update your profile first to use the registrations feature!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        RegistrationsButton.IsEnabled = true;
                    }
                    break;
                case "AreaLeader":
                    UsersButton.IsEnabled = false;
                    LogsButton.IsEnabled = false;
                    ProfileButton.Visibility = Visibility.Hidden;
                    RegistrationsButton.IsEnabled = false;
                    MemberManageButton.IsEnabled = false;
                    break;
                case "Police":
                    UsersButton.IsEnabled = false;
                    ProfileButton.Visibility = Visibility.Hidden;
                    RegistrationsButton.IsEnabled = false;
                    break;
                case "Administrator":
                    HouseholdsButton.IsEnabled = false;
                    LogsButton.IsEnabled = false;
                    NotificationsButton.IsEnabled = false;
                    RegistrationsButton.IsEnabled = false;
                    RegistrationsManageButton.IsEnabled = false;
                    ProfileButton.Visibility = Visibility.Hidden;
                    MemberManageButton.IsEnabled = false;
                    // All buttons enabled
                    break;
            }
        }

        public void UpdateProfileStatus()
        {
            if (!_currentUser.Cccd.IsNullOrEmpty() && !_currentUser.Address.IsNullOrEmpty())
            {
                RegistrationsButton.IsEnabled = true;
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
            RegistrationsManageWindow window = new RegistrationsManageWindow(_currentUser);
            window.Show();
        }
        private void RegistrationsButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationsWindow wndow = new RegistrationsWindow(_currentUser);
            wndow.Show();
        }

        private void NotificationsButton_Click(object sender, RoutedEventArgs e)
        {
            Notifications window = new Notifications(_currentUser);
            window.Show();
        }

        private void LogsButton_Click(object sender, RoutedEventArgs e)
        {
            Logs window = new Logs(_currentUser);
            window.Show();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            UserProfileWindow window = new UserProfileWindow(_currentUser);
            window.Show();
        }

        private void NotificationViewButton_Click(object sender, RoutedEventArgs e)
        {
            NotificationView window = new NotificationView(_currentUser);
            window.Show();
        }

        private void MembersManageButton_Click(object sender, RoutedEventArgs e)
        {
            MemberWindow window = new MemberWindow();
            window.Show();
        }
    }
}
