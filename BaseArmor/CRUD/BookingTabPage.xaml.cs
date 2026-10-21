using BaseArmor.Pages;
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
    /// Логика взаимодействия для BookingTabPage.xaml
    /// </summary>
    public partial class BookingTabPage : Page
    {
        public BookingTabPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            DataGridBooking.ItemsSource = Core.Context.Booking.OrderByDescending(b => b.start_datatime).ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddBookingPage(null));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var booking = button.DataContext as Booking;
            if (booking == null) return;

            NavigationService?.Navigate(new AddBookingPage(booking));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = DataGridBooking.SelectedItems.Cast<Booking>().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Выберите брони для удаления.");
                return;
            }

            if (MessageBox.Show($"Удалить {selected.Count} броней?", "Внимание",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Booking.RemoveRange(selected);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _7AdminPage());
        }
    }
}
