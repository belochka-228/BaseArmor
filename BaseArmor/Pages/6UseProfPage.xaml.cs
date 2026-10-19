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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BaseArmor.Pages
{
    /// <summary>
    /// Личный кабинет пользователя, со списком бронирований и листом ожидания.
    /// </summary>
    public partial class _6UseProfPage : Page
    {
        public _6UseProfPage()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserData.CurrentUser == null)
            {
                MessageBox.Show("Пользователь не авторизован.");
                NavigationService?.Navigate(new _1LoginPage());
                return;
            }
            LoadUserInfo();
            LoadBookings();
            LoadWaitlist();
        }

        /// <summary>
        /// Информация о пользователе
        /// </summary>
        private void LoadUserInfo()
        {
            var u = UserData.CurrentUser;
            txtFIO.Text = $"{u.last_name} {u.first_name}";
            txtPhone.Text = u.phone;
            txtLogin.Text = u.login;
            txtRole.Text = u.Roles?.name ?? "—";
        }

        /// <summary>
        /// Мои брони
        /// </summary>
        private void LoadBookings()
        {
            var bookings = Core.Context.Booking.Where(b => b.user_id == UserData.CurrentUser.id).OrderByDescending(b => b.start_datatime).ToList();
            LstMyBroni.ItemsSource = bookings;
        }

        /// <summary>
        /// Мои ожидания
        /// </summary>
        private void LoadWaitlist()
        {
            var waitlist = Core.Context.Waitlist.Where(w => w.user_id == UserData.CurrentUser.id).OrderByDescending(w => w.desired_datetime).ToList();
            foreach (var w in waitlist)
            {
                w.CanBook = CheckAvailableTables(w);
            }
            LstMyExpectations.ItemsSource = null;
            LstMyExpectations.ItemsSource = waitlist;
        }

        /// <summary>
        /// Отмена брони
        /// </summary>
        private void BtnOtmenaBroni_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var booking = button.DataContext as Booking;

            if (booking == null) return;

            if (booking.status_id == 3 || booking.status_id == 4)
            {
                MessageBox.Show("Эту бронь нельзя отменить.");
                return;
            }

            if (MessageBox.Show("Отменить бронь?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var dbBooking = Core.Context.Booking.FirstOrDefault(b => b.id == booking.id);
                if (dbBooking != null)
                {
                    dbBooking.status_id = 3;
                    Core.Context.SaveChanges();
                }

                MessageBox.Show("Бронь отменена.");
                LoadBookings();
                LoadWaitlist();
            }
        }

        /// <summary>
        /// Удаление из Waitlist
        /// </summary>
        private void RemoveWaitlist_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var waitlist = button.DataContext as Waitlist;

            if (waitlist == null) return;

            if (MessageBox.Show("Удалить из листа ожидания?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var dbWaitlist = Core.Context.Waitlist.FirstOrDefault(w => w.id == waitlist.id);
                if (dbWaitlist != null)
                {
                    Core.Context.Waitlist.Remove(dbWaitlist);
                    Core.Context.SaveChanges();
                }

                MessageBox.Show("Удалено из листа ожидания.");
                LoadWaitlist();
            }
        }
        private bool CheckAvailableTables(Waitlist w)
        {
            var start = w.desired_datetime;
            var end = start.AddMinutes(120);

            var allBooking = Core.Context.Booking.Where(b => b.status_id == 1 || b.status_id == 2).ToList();
            var busyTableIds = allBooking.Where(b => b.start_datatime <= end && b.start_datatime.AddMinutes(b.duration_minutes) >= start).Select(b => b.table_id).ToList();
            var table = Core.Context.DiningTable.Where(t => t.is_active == true).Where(t => !busyTableIds.Contains(t.id))
                .Where(t => t.TypeTable.count_sit == w.guests_count).Where(t => t.Halls.restaurant_id == w.restaurant_id).FirstOrDefault();

            return table != null;
        }
        private void BookFromWaitlist_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var waitlist = button.DataContext as Waitlist;
            if (waitlist == null) return;

            var start = waitlist.desired_datetime;
            var end = start.AddMinutes(120);

            var allBooking = Core.Context.Booking.Where(b => b.status_id == 1 || b.status_id == 2).ToList();
            var busyTableIds = allBooking.Where(b => b.start_datatime <= end
                         && b.start_datatime.AddMinutes(b.duration_minutes) >= start).Select(b => b.table_id).ToList();
            var table = Core.Context.DiningTable.Where(t => t.is_active == true).Where(t => !busyTableIds.Contains(t.id))
                .Where(t => t.TypeTable.count_sit == waitlist.guests_count).Where(t => t.Halls.restaurant_id == waitlist.restaurant_id).FirstOrDefault();

            if (table == null)
            {
                MessageBox.Show("Свободных столов нет.");
                return;
            }

            var booking = new Booking
            {
                user_id = UserData.CurrentUser.id,
                table_id = table.id,
                start_datatime = start,
                duration_minutes = 120,
                guests_count = waitlist.guests_count,
                status_id = 1
            };
            Core.Context.Booking.Add(booking);

            Core.Context.Waitlist.Remove(waitlist);
            Core.Context.SaveChanges();

            MessageBox.Show("Бронь создана из листа ожидания!");
            LoadBookings();
            LoadWaitlist();
        }
        private void BtnRest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _3RestoranPage());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            UserData.CurrentUser = null;
            UserData.SelectedRestaurant = null;
            UserData.SelectedHall = null;
            NavigationService?.Navigate(new _1LoginPage());
        }
    }
} 

