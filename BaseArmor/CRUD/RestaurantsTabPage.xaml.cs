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
    /// Логика взаимодействия для RestaurantsTabPage.xaml
    /// </summary>
    public partial class RestaurantsTabPage : Page
    {
        public RestaurantsTabPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            DataGridRestaurants.ItemsSource = Core.Context.Restaurants.ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddRestaurantPage(null));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var restaurant = button.DataContext as Restaurants;
            if (restaurant == null) return;

            NavigationService?.Navigate(new AddRestaurantPage(restaurant));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedRestaurants = DataGridRestaurants.SelectedItems.Cast<Restaurants>().ToList();
            if (selectedRestaurants.Count == 0)
            {
                MessageBox.Show("Выберите рестораны для удаления.");
                return;
            }

            if (MessageBox.Show($"Удалить {selectedRestaurants.Count} ресторанов со всеми залами и бронями?", "Внимание",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (var restaurant in selectedRestaurants)
                    {
                        var halls = Core.Context.Halls.Where(h => h.restaurant_id == restaurant.id).ToList();

                        foreach (var hall in halls)
                        {
                            var tables = Core.Context.DiningTable.Where(t => t.hall_id == hall.id).ToList();

                            foreach (var table in tables)
                            {
                                var bookings = Core.Context.Booking.Where(b => b.table_id == table.id).ToList();

                                foreach (var booking in bookings)
                                {
                                    var deposit = Core.Context.Deposit.FirstOrDefault(d => d.id == booking.id);
                                    if (deposit != null)
                                        Core.Context.Deposit.Remove(deposit);
                                }
                                Core.Context.Booking.RemoveRange(bookings);
                            }
                            Core.Context.DiningTable.RemoveRange(tables);
                        }

                        Core.Context.Halls.RemoveRange(halls);

                        var waitlists = Core.Context.Waitlist.Where(w => w.restaurant_id == restaurant.id).ToList();
                        Core.Context.Waitlist.RemoveRange(waitlists);
                    }
                    Core.Context.Restaurants.RemoveRange(selectedRestaurants);
                    Core.Context.SaveChanges();

                    MessageBox.Show("Рестораны и все связанные данные удалены.");
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
