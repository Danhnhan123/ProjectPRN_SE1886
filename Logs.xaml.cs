using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectPRN_SE1886;

namespace ProjectPRN_SE1886
{
    public partial class Logs : Window
    {
        private readonly DAO.LogDAO _logDAO;

        public Logs(Models.User currentUser)
        {
            InitializeComponent();
            _logDAO = new DAO.LogDAO(); 
            LoadLogs();
        }

        private void LoadLogs()
        {
            dgLogs.ItemsSource = _logDAO.GetAllLogs();
        }

        private void SearchLogs(object sender, RoutedEventArgs e)
        {
            int? userIdFilter = string.IsNullOrEmpty(txtUserId.Text) ? (int?)null : int.Parse(txtUserId.Text);
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;

            var logs = _logDAO.GetAllLogs().AsQueryable();

            if (userIdFilter.HasValue)
                logs = logs.Where(l => l.UserId == userIdFilter.Value);

            if (startDate.HasValue)
                logs = logs.Where(l => l.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                logs = logs.Where(l => l.Timestamp <= endDate.Value);

            dgLogs.ItemsSource = logs.OrderByDescending(l => l.Timestamp).ToList();
        }

        private void DeleteLogs(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa toàn bộ lịch sử?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _logDAO.DeleteAllLogs();
                LoadLogs();
            }
        }

        private void RefreshLogs(object sender, RoutedEventArgs e)
        {
            LoadLogs();
        }

        private void txtUserId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchLogs(sender, e);
            }
        }
    }
}
