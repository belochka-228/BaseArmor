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
    /// Логика взаимодействия для _5BookingPage.xaml
    /// </summary>
    public partial class _5BookingPage : Page
    {
        public _5BookingPage()
        {
            InitializeComponent();
            if(UserData.SelectedRestaurant == null || UserData.SelectedHall == null)
            {
                MessageBox.Show("Не выбран ресторан или зал");
                NavigationService?.Navigate(new _3RestoranPage());
                return;
                LoadInfo();
            }
        }
        private void LoadInfo()
        {
            var r = UserData.SelectedRestaurant;
            var h = UserData.SelectedHall;

            txtRestaurantName.Text = r.name;
            //imgRestaurant.Source 
            txtRestaurantAddress.Text = $"Адрес: {r.address}";
            txtRestaurantRating.Text = $"Рейтинг: {r.rating}";
            txtRestaurantCheck.Text = $"Средний чек: {r.average_check} ₽";
            txtRestaurantDescription.Text = r.description;

            txtHallName.Text = h.name;
            //imgHall.Source 
            txtHallDeposit.Text = $"Депозит: {(h.has_deposit ? "Да" : "Нет")}";
            txtHallCapacity.Text = $"Вместимость: {h.capacity} чел.";
            txtHallDescription.Text = h.description;
        }
        private void BtnHalls_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            UserData.SelectedHall = null;
            NavigationService?.Navigate(new _4HallPage());
        }

        private void txtTime_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtTimeHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void txtDurationHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void txtGuestsHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtDuration_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtGuests_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
