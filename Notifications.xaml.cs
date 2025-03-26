using ProjectPRN_SE1886.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectPRN_SE1886
{
    public partial class Notifications : Window
    {
        private List<Notification> _allNotifications;

        public Notifications(User currentUser)
        {
            InitializeComponent();
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            try
            {
                using (var context = new PrnProjectContext())
                {
                    int currentUserId = 1;

                    var notifications = context.Notifications
                        .Where(n => n.UserId == currentUserId)
                        .OrderByDescending(n => n.SentDate)
                        .ToList();

                    if (notifications.Count == 0)
                    {
                        MessageBox.Show("Không có thông báo nào cho người dùng.");
                    }
                    else
                    {
                        Console.WriteLine($"Có {notifications.Count} thông báo.");
                        foreach (var noti in notifications)
                        {
                            Console.WriteLine($"{noti.SentDate} - {noti.Message} - Đã đọc: {noti.IsRead}");
                        }
                    }

                    myListView.ItemsSource = null;
                    myListView.ItemsSource = notifications;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load thông báo: " + ex.Message);
            }




        }

    }
}
