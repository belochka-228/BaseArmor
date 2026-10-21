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
            var selected = DataGridRestaurants.SelectedItems.Cast<Restaurants>().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Выберите рестораны для удаления.");
                return;
            }

            if (MessageBox.Show($"Удалить {selected.Count} ресторанов?", "Внимание",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Restaurants.RemoveRange(selected);
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
