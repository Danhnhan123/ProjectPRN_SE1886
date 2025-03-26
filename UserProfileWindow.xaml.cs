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
using ProjectPRN_SE1886.viewModel;

namespace ProjectPRN_SE1886
{
    /// <summary>
    /// Interaction logic for UserProfileWindow.xaml
    /// </summary>
    public partial class UserProfileWindow : Window
    {
        private User _currentUser;
        public UserProfileWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            FullnameTextBox.Text = _currentUser.FullName;
            PasswordTextBox.Text = _currentUser.Password;
            EmailTextBox.Text = _currentUser.Email;
            txtAddress.Text = _currentUser.Address;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(FullnameTextBox.Text) &&
        !string.IsNullOrEmpty(PasswordTextBox.Text) &&
        !string.IsNullOrEmpty(EmailTextBox.Text) &&
        !string.IsNullOrEmpty(txtAddress.Text))
            {
                _currentUser.FullName = FullnameTextBox.Text;
                _currentUser.Password = PasswordTextBox.Text;
                _currentUser.Email = EmailTextBox.Text;
                _currentUser.Address = txtAddress.Text;

                DAO.UserDAO.UpdateUser(_currentUser);

                MessageBox.Show("Update profile successfully!");
            }
            else
            {
                MessageBox.Show("Please fill in all fields!");
            }
        }
    }
}
