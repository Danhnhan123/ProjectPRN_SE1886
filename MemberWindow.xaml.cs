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
using ProjectPRN_SE1886.viewModel;

namespace ProjectPRN_SE1886
{
    /// <summary>
    /// Interaction logic for MemberWindow.xaml
    /// </summary>
    public partial class MemberWindow : Window
    {
        public MemberWindow()
        {
            InitializeComponent();
            LoadComboBox();
        }

        public void LoadComboBox()
        {
            RoleComboBox.Items.Add("All");
            var households = DAOs.HouseholdDAO.GetAllHouseholds();
            foreach (var household in households)
            {
                RoleComboBox.Items.Add(household.HouseholdNumber);
            }
            RoleComboBox.SelectedIndex = 0;
        }

        public void LoadInitialData2()
        {
            var members = MemberDAO.GetAllHouseholds();
            if (RoleComboBox.SelectedItem != null)
            {
                if (RoleComboBox.SelectedItem.ToString().Equals("All"))
                {

                    UserDataGrid.ItemsSource = members;
                }
                else
                {
                    string role = RoleComboBox.SelectedItem.ToString();
                    members = MemberDAO.GetHouseholdByHouseholdNumber(role, members);
                    UserDataGrid.ItemsSource = members;
                }
            }
            if (FullnameSearchTextBox != null && !FullnameSearchTextBox.Text.IsNullOrEmpty())
            {
                string address = FullnameSearchTextBox.Text;
                members = MemberDAO.GetMemberByFullName(address, members);
                UserDataGrid.ItemsSource = members;
            }
            if (RelationshipSearchTextBox != null && !RelationshipSearchTextBox.Text.IsNullOrEmpty())
            {
                string address = RelationshipSearchTextBox.Text;
                members = MemberDAO.GetMemberByRelationship(address, members);
                UserDataGrid.ItemsSource = members;
            }
        }

        private void cbRole_selection(object sender, SelectionChangedEventArgs e)
        {
            LoadInitialData2();
        }

        private void txtname_keydown(object sender, KeyEventArgs e)
        {
            LoadInitialData2();
        }

        private void txtAddress_keydown(object sender, KeyEventArgs e)
        {
            LoadInitialData2();
        }
    }
}
