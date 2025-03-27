using Microsoft.EntityFrameworkCore;
using ProjectPRN_SE1886.Models;
using ProjectPRN_SE1886;
using System.Windows.Controls;
using System.Windows;
namespace ProjectPRN_SE1886
{
    public partial class RegistrationsManageWindow : Window
    {
        private readonly RegistrationsDAO _registrationsDAO;
        private readonly User _currentUser;
        private List<Registration> _pendingRegistrations;
        private List<Registration> _historyRegistrations;

        public RegistrationsManageWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _registrationsDAO = new RegistrationsDAO();
            LoadData();
        }

        private void LoadData()
        {
            _pendingRegistrations = _registrationsDAO.GetPendingRegistrations();
            PendingRegistrationsDataGrid.ItemsSource = _pendingRegistrations;
            _historyRegistrations = _registrationsDAO.GetProcessedRegistrations();
            HistoryRegistrationsDataGrid.ItemsSource = _historyRegistrations;
        }

        private void PendingRegistrationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PendingRegistrationsDataGrid.SelectedItem is Registration selected)
            {
                DetailFullName.Text = $"Name: {selected.User?.FullName ?? "N/A"}";
                DetailEmail.Text = $"Email: {selected.User?.Email ?? "N/A"}";
                DetailCCCD.Text = $"CCCD: {selected.User?.Cccd ?? "N/A"}";
                Details.Text = $"Head of Household: {selected.Household?.HeadOfHousehold?.FullName ?? "N/A"}, CCCD: {selected.Household?.HeadOfHousehold?.Cccd ?? "N/A"}\nAddress: {selected.Household?.Address ?? "N/A"}"; 
                DetailType.Text = $"Type: {selected.RegistrationType}";
                DetailStartDate.Text = $"Start Date: {selected.StartDate}";
                DetailEndDate.Text = $"End Date: {selected.EndDate ?? DateOnly.MinValue}";
                DetailStatus.Text = $"Status: {selected.Status}";
                DetailComments.Text = $"Comments: {selected.Comments}";
            }
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
{
    if (PendingRegistrationsDataGrid.SelectedItem is Registration selected && 
        (!string.IsNullOrEmpty(RelationshipTextBox.Text) || selected.RegistrationType == "MoveOut"))
    {
        selected.Status = "Approved";
        selected.ApprovedBy = _currentUser.UserId;
        
        if (selected.RegistrationType == "MoveOut")
        {
            var member = _registrationsDAO.GetHouseholdMemberByUserId(selected.UserId.Value);
            if (member != null && member.HouseholdId == selected.HouseholdId)
            {
                _registrationsDAO.RemoveHouseholdMember(member); // Use DAO to remove the member
            }
            MessageBox.Show("Registration approved and user removed from household!");
        }
        else
        {
            var member = _registrationsDAO.GetHouseholdMemberByUserId(selected.UserId.Value);
            if (member != null)
            {
                member.HouseholdId = selected.HouseholdId;
                member.Relationship = RelationshipTextBox.Text;
                _registrationsDAO.UpdateHouseholdMember(member);
            }
            else
            {
                var newMember = new HouseholdMember
                {
                    HouseholdId = selected.HouseholdId,
                    UserId = selected.UserId.Value,
                    Relationship = RelationshipTextBox.Text
                };
                _registrationsDAO.AddHouseholdMember(newMember);
            }
            MessageBox.Show("Registration approved and user added to household!");
        }

        _registrationsDAO.UpdateRegistration(selected);
        LoadData();
        ClearDetails();
    }
    else
    {
        MessageBox.Show("Please select a registration and enter a relationship (except for MoveOut type).");
    }
}
        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (PendingRegistrationsDataGrid.SelectedItem is Registration selected)
            {
                selected.Status = "Rejected";
                selected.ApprovedBy = _currentUser.UserId;
                _registrationsDAO.UpdateRegistration(selected);
                MessageBox.Show("Registration rejected!");
                LoadData();
                ClearDetails();
            }
            else
            {
                MessageBox.Show("Please select a registration.");
            }
        }

        private void ClearDetails()
        {
            DetailFullName.Text = "";
            DetailEmail.Text = "";
            Details.Text = "";
            DetailType.Text = "";
            DetailStartDate.Text = "";
            DetailEndDate.Text = "";
            DetailStatus.Text = "";
            DetailComments.Text = "";
            RelationshipTextBox.Text = "";
        }

        private void PendingSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = PendingSearchTextBox.Text.ToLower();
            var filtered = _pendingRegistrations.Where(r =>
                (r.User?.FullName?.ToLower().Contains(searchText) ?? false) ||
                (r.User?.Email?.ToLower().Contains(searchText) ?? false) ||
                (r.Household?.HeadOfHousehold?.FullName?.ToLower().Contains(searchText) ?? false) ||
                (r.Household?.Address?.ToLower().Contains(searchText) ?? false)).ToList();
            PendingRegistrationsDataGrid.ItemsSource = filtered;
        }

        private void HistorySearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = HistorySearchTextBox.Text.ToLower();
            var filtered = _historyRegistrations.Where(r =>
                (r.User?.FullName?.ToLower().Contains(searchText) ?? false) ||
                (r.User?.Email?.ToLower().Contains(searchText) ?? false) ||
                (r.Household?.Address?.ToLower().Contains(searchText) ?? false)).ToList();
            HistoryRegistrationsDataGrid.ItemsSource = filtered;
        }
    }
}