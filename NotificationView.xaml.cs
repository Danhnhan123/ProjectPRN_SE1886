using System.Windows;
using ProjectPRN_SE1886.Models;

namespace ProjectPRN_SE1886
{
    public partial class NotificationView : Window
    {
        public NotificationView(User currentUser)
        {
            InitializeComponent();
            DataContext = new NotificationViewModel(currentUser);
        }
    }
}
