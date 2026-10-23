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

namespace BaseArmor.CRUD
{
    /// <summary>
    /// Логика взаимодействия для AddBookingPage.xaml
    /// </summary>
    public partial class AddBookingPage : Page
    {
        private Booking _currentBooking;
        private bool _isEdit = false;

        public AddBookingPage(Booking booking)
        {
            InitializeComponent();

            _isEdit = booking != null;
            _currentBooking = booking ?? new Booking();

            cmbUser.ItemsSource = Core.Context.Users.ToList();
            cmbStatus.ItemsSource = Core.Context.Status.ToList();
            cmbRestaurant.ItemsSource = Core.Context.Restaurants.ToList();

            DataContext = _currentBooking;

            if (_isEdit)
            {
                txtDate.Text = _currentBooking.start_datatime.ToString("dd.MM.yyyy HH:mm");
                if (_currentBooking.DiningTable != null && _currentBooking.DiningTable.Halls != null)
                {
                    cmbRestaurant.SelectedValue = _currentBooking.DiningTable.Halls.restaurant_id;
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (cmbUser.SelectedValue == null)
                errors.AppendLine("Выберите пользователя.");
            if (cmbTable.SelectedValue == null)
                errors.AppendLine("Выберите столик.");
            if (cmbStatus.SelectedValue == null)
                errors.AppendLine("Выберите статус.");

            if (!DateTime.TryParse(txtDate.Text, out DateTime parsedDate))
            {
                errors.AppendLine("Неверный формат даты. Используйте дд.мм.гггг чч:мм");
            }
            else
            {
                _currentBooking.start_datatime = parsedDate;
            }

            if (_currentBooking.duration_minutes <= 0)
                errors.AppendLine("Укажите длительность.");
            if (_currentBooking.guests_count <= 0)
                errors.AppendLine("Укажите количество гостей.");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка");
                return;
            }

            int userId = (int)cmbUser.SelectedValue;
            int tableId = (int)cmbTable.SelectedValue;
            int statusId = (int)cmbStatus.SelectedValue;

            DateTime start = _currentBooking.start_datatime;
            DateTime end = start.AddMinutes(_currentBooking.duration_minutes);

            if (start < DateTime.Now)
            {
                MessageBox.Show("Нельзя создать бронь на прошедшее время.", "Ошибка");
                return;
            }

            var table = Core.Context.DiningTable.FirstOrDefault(t => t.id == tableId);
            if (table != null && table.TypeTable != null)
            {
                if (_currentBooking.guests_count > table.TypeTable.count_sit)
                {
                    MessageBox.Show($"Стол вмещает не более {table.TypeTable.count_sit} гостей.", "Ошибка");
                    return;
                }
            }

            var allBookings = Core.Context.Booking.Where(b => b.status_id != 3 && b.status_id != 4).Where(b => b.id != _currentBooking.id).ToList();

            var busyTableIds = allBookings.Where(b => b.start_datatime <= end
                         && b.start_datatime.AddMinutes(b.duration_minutes) >= start).Select(b => b.table_id).ToList();
            if (busyTableIds.Contains(tableId))
            {
                MessageBox.Show("Этот стол уже занят на выбранное время.", "Ошибка");
                return;
            }

            var userOverlap = allBookings.Where(b => b.user_id == userId).Any(b => b.start_datatime <= end
                       && b.start_datatime.AddMinutes(b.duration_minutes) >= start);
            if (userOverlap)
            {
                MessageBox.Show("У пользователя уже есть бронь на это время.", "Ошибка");
                return;
            }

            try
            {
                _currentBooking.user_id = userId;
                _currentBooking.table_id = tableId;
                _currentBooking.status_id = statusId;

                if (!_isEdit)
                    Core.Context.Booking.Add(_currentBooking);

                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.Navigate(new BookingTabPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new BookingTabPage());
        }

        private void cmbRestaurant_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRestaurant.SelectedValue == null) return;
            int restaurantId = (int)cmbRestaurant.SelectedValue;

            var tables = Core.Context.DiningTable.Where(t => t.is_active == true).Where(t => t.Halls.restaurant_id == restaurantId).ToList();

            cmbTable.ItemsSource = tables;
            if (_isEdit && _currentBooking.table_id > 0)
            {
                cmbTable.SelectedValue = _currentBooking.table_id;
            }
        }
    }
}
