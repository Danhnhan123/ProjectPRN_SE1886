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
using ProjectPRN_SE1886.Models;

namespace ProjectPRN_SE1886
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadHouseholds();
        }

        private void LoadHouseholds()
        {
            var households = householdDAO.GetHouseholds();
            DataGridResidents.ItemsSource = households;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement search logic here
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement add logic here
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement edit logic here
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement delete logic here
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement sort logic here
        }
    }
}