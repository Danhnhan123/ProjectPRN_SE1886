using System.Collections.ObjectModel;
using System.Text;
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
using ProjectPRN_SE1886.viewModel;

namespace ProjectPRN_SE1886
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly DAO.UserDAO _userDAO;

        public MainWindow()
        {
            InitializeComponent();
            _userDAO = new DAO.UserDAO();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            User user = _userDAO.Login(email, password);

            if (user != null)
            {
                // Role được lấy từ database, không cần chọn từ ComboBox
                DashboardWindow dashboard = new DashboardWindow(user);
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid email or password");
            }
        }

        
    }

}