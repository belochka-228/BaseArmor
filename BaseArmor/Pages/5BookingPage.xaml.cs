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
using System.IO;

namespace BaseArmor.Pages
{
    /// <summary>
    /// Логика взаимодействия для _5BookingPage.xaml
    /// </summary>
    public partial class _5BookingPage : Page
    {
        public _5BookingPage()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserData.CurrentUser == null)
            {
                MessageBox.Show("Пользователь не авторизован. Войдите заново.");
                NavigationService?.Navigate(new _1LoginPage());
                return;
            }

            if (UserData.SelectedRestaurant == null || UserData.SelectedHall == null)
            {
                MessageBox.Show("Не выбран ресторан или зал");
                NavigationService?.Navigate(new _3RestoranPage());
                return;
            }

            LoadInfo();
        }
        private void LoadInfo()
        {
            var r = UserData.SelectedRestaurant;
            var h = UserData.SelectedHall;

            txtRestaurantName.Text = r.name;
            txtRestaurantAddress.Text = $"Адрес: {r.address}";
            txtRestaurantRating.Text = $"Рейтинг: {r.rating}";
            txtRestaurantCheck.Text = $"Средний чек: {r.average_check} ₽";
            txtRestaurantDescription.Text = r.description;

            string fileName = System.IO.Path.GetFileName(r.photo_url);
            imgRestaurant.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{fileName}"));

            txtHallName.Text = h.name;
            txtHallDeposit.Text = $"Депозит: {(h.has_deposit ? "Да" : "Нет")}";
            txtHallCapacity.Text = $"Вместимость: {h.capacity} чел.";
            txtHallDescription.Text = h.description;

            string fileNameHall = System.IO.Path.GetFileName(h.photo_url);
            imgHall.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{fileNameHall}"));
        }
        private void txtTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtTimeHint.Visibility = string.IsNullOrEmpty(txtTime.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtTimeHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtTime.Focus();
        }

        private void txtDuration_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtDurationHint.Visibility = string.IsNullOrEmpty(txtDuration.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtDurationHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtDuration.Focus();
        }

        private void txtGuests_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtGuestsHint.Visibility = string.IsNullOrEmpty(txtGuests.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void txtGuestsHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtGuests.Focus();
        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtTime.Text) || dpDate.SelectedDate == null || string.IsNullOrWhiteSpace(txtDuration.Text)
                || string.IsNullOrWhiteSpace(txtGuests.Text))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            if(!TimeSpan.TryParse(txtTime.Text, out TimeSpan time))
            {
                MessageBox.Show("Неверный формат времени, пример 18:00");
                return;
            }
            if(!int.TryParse(txtDuration.Text, out int duration) || duration <= 0)
            {
                MessageBox.Show("Неверно введена продолжительность брони!");
                return;
            }
            if (duration > 480)
            {
                MessageBox.Show("Длительность не может превышать 8 часов.");
                return;
            }
            if (!int.TryParse(txtGuests.Text, out int guest) || guest <= 0)
            {
                MessageBox.Show("Неверно введено количество гостей!");
                return;
            }
            if(UserData.SelectedHall.capacity.HasValue && guest > UserData.SelectedHall.capacity.Value)
            {
                MessageBox.Show($"Зал вмещает не более {UserData.SelectedHall.capacity} человек!");
                return;
            }

            DateTime startDateTime = dpDate.SelectedDate.Value.Date + time;
            DateTime endDateTime = startDateTime.AddMinutes(duration);

            if (startDateTime < DateTime.Now)
            {
                MessageBox.Show("Нельзя забронировать в прошлом!");
                return;
            }
            var userBookings = Core.Context.Booking.Where(b => b.user_id == UserData.CurrentUser.id)
            .Where(b => b.status_id == 1 || b.status_id == 2).ToList();

            var overlap = userBookings.Any(b =>
                b.start_datatime < endDateTime && b.start_datatime.AddMinutes(b.duration_minutes) > startDateTime);

            if (overlap)
            {
                MessageBox.Show("У вас уже есть бронь на это время. Нельзя быть в двух местах одновременно.");
                return;
            }

            var allBooking = Core.Context.Booking.Where(b => b.status_id == 1 || b.status_id == 2).ToList();

            var BusyTable = allBooking.Where(b => b.start_datatime <= endDateTime
                && b.start_datatime.AddMinutes(b.duration_minutes) >= startDateTime)
                .Select(b => b.table_id).ToList();

            var table = Core.Context.DiningTable
            .Where(t => t.hall_id == UserData.SelectedHall.id).Where(t => !BusyTable.Contains(t.id))
            .Where(t => t.TypeTable.count_sit == guest).Where(t => t.is_active == true).FirstOrDefault();

            if (table == null)
            {
                var existingWaitlist = Core.Context.Waitlist.FirstOrDefault(w => w.user_id == UserData.CurrentUser.id
                              && w.restaurant_id == UserData.SelectedRestaurant.id
                              && w.desired_datetime == startDateTime);

                if (existingWaitlist != null)
                {
                    MessageBox.Show("Вы уже в листе ожидания на это время.");
                    return;
                }

                var waitlist = new Waitlist
                {
                    user_id = UserData.CurrentUser.id,
                    restaurant_id = UserData.SelectedRestaurant.id,
                    guests_count = guest,
                    desired_datetime = startDateTime
                };

                Core.Context.Waitlist.Add(waitlist);
                Core.Context.SaveChanges();

                MessageBox.Show("Свободных столов на это количество гостей нет. Вы добавлены в лист ожидания.");
                NavigationService?.Navigate(new _6UseProfPage());
                return;
            }

            var booking = new Booking
            {
                user_id = UserData.CurrentUser.id,
                table_id = table.id,
                start_datatime = startDateTime,
                duration_minutes = duration,
                guests_count = guest,
                status_id = 1,
                comment = txtComment.Text
            };
            Core.Context.Booking.Add(booking);
            Core.Context.SaveChanges();

            if (UserData.SelectedHall.has_deposit)
            {
                MessageBox.Show("Создаю депозит...");
                var depozit = new Deposit
                {
                    id = booking.id,
                    amount = 1500,
                    status_id = 1,
                    method_id = 1,
                    payment_date = DateTime.Now
                };
                Core.Context.Deposit.Add(depozit);
                Core.Context.SaveChanges();

                UserData.CurrentDepozit = depozit;

                MessageBox.Show("Бронь успешно создана, требуется оплата депозита!");
                NavigationService?.Navigate(new _9DepozitPay());
            }
            else
            {
                MessageBox.Show("Бронь успешно создана!");
                NavigationService?.Navigate(new _6UseProfPage());
            }
        }
        private void BtnHalls_Click(object sender, RoutedEventArgs e)
        {
            UserData.SelectedHall = null;
            NavigationService?.Navigate(new _4HallPage());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            UserData.SelectedRestaurant = null;
            UserData.SelectedHall = null;
            UserData.CurrentUser = null;
            NavigationService?.Navigate(new _1LoginPage());
        }
    }
}
