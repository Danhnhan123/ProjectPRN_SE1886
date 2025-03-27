using ProjectPRN_SE1886.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using ProjectPRN_SE1886.viewModel;

namespace ProjectPRN_SE1886
{
    public class NotificationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Notification> Notifications { get; set; }
        public Notification SelectedNotification { get; set; }

        public ObservableCollection<User> Users { get; set; }
        public User SelectedUser { get; set; }

        public string NewMessage { get; set; }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public NotificationViewModel(User currentUser)
        {
            LoadNotifications();
            LoadUsers();

            AddCommand = new RelayCommand(AddNotification);
            EditCommand = new RelayCommand(EditNotification, CanModify);
            DeleteCommand = new RelayCommand(DeleteNotification, CanModify);
        }

        void LoadNotifications()
        {
            using var context = new PrnProjectContext();
            var list = context.Notifications
                              .Include(n => n.User)
                              .OrderByDescending(n => n.SentDate)
                              .ToList();

            Notifications = new ObservableCollection<Notification>(list);
        }

        void LoadUsers()
        {
            using var context = new PrnProjectContext();
            Users = new ObservableCollection<User>(context.Users.ToList());
        }

        void AddNotification()
        {
            if (SelectedUser == null || string.IsNullOrWhiteSpace(NewMessage))
            {
                MessageBox.Show("Vui lòng chọn người nhận và nhập nội dung.");
                return;
            }

            var newNoti = new Notification
            {
                Message = NewMessage,
                UserId = SelectedUser.UserId,
                SentDate = System.DateTime.Now,
                IsRead = false
            };

            using var context = new PrnProjectContext();
            context.Notifications.Add(newNoti);
            context.SaveChanges();

            // 👉 Sau khi lưu xong, gán User lại để binding trong UI
            newNoti.User = SelectedUser;

            Notifications.Add(newNoti);
            MessageBox.Show("Gửi thông báo thành công!");

            // Reset input
            NewMessage = string.Empty;
            SelectedUser = null;
            OnPropertyChanged(nameof(NewMessage));
            OnPropertyChanged(nameof(SelectedUser));
        }



        void EditNotification()
        {
            if (SelectedNotification == null) return;

            using var context = new PrnProjectContext();
            context.Notifications.Update(SelectedNotification);
            context.SaveChanges();

            MessageBox.Show("Đã cập nhật thông báo.");
        }

        void DeleteNotification()
        {
            if (SelectedNotification == null) return;

            var result = MessageBox.Show("Bạn có chắc chắn muốn xoá thông báo này?", "Xác nhận", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            using var context = new PrnProjectContext();
            context.Notifications.Remove(SelectedNotification);
            context.SaveChanges();

            Notifications.Remove(SelectedNotification);
            MessageBox.Show("Đã xoá thông báo.");
        }

        bool CanModify() => SelectedNotification != null;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
