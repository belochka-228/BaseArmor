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
    /// Логика взаимодействия для _4HallPage.xaml
    /// </summary>
    public partial class _4HallPage : Page
    {
        private bool _isLoaded = false;
        public _4HallPage()
        {
            InitializeComponent();
            
            if(UserData.SelectedRestaurant == null)
            {
                Console.WriteLine("Ресторан не выбран");
                NavigationService?.Navigate(new _3RestoranPage());
                 return;
            }
            switch (UserData.DepositFilter)
            {
                case 0: rbAll.IsChecked = true;
                    break;
                case 1: rbWithDeposit.IsChecked = true;
                    break;
                case 2: rbWithoutDeposit.IsChecked = true;
                    break;
            }
            _isLoaded = true;
            TitleRestik.Text = $"Залы ресторана: {UserData.SelectedRestaurant.name}";
            LoadHalls();
        }
        private void LoadHalls()
        {
            var restik = UserData.SelectedRestaurant.id;
            var hall = Core.Context.Halls.Where(h => h.restaurant_id == restik).Where(h => h.is_active == true);

            var searce = txtSource.Text.Trim().ToLower();
            if(!string.IsNullOrWhiteSpace(searce) ) 
                hall = hall.Where(h => h.name.ToLower().Contains(searce));

            if (rbWithDeposit.IsChecked == true)
                hall = hall.Where(h => h.has_deposit == true);
            else if (rbWithoutDeposit.IsChecked == true)
                hall = hall.Where(h => h.has_deposit == false);

            ListHalls.ItemsSource = hall.ToList();
        }

        private void txtSource_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded) return;
            LoadHalls();
        }

        private void rb_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded) return;

            if (rbAll.IsChecked == true) UserData.DepositFilter = 0;
            else if (rbWithDeposit.IsChecked == true) UserData.DepositFilter = 1;
            else if (rbWithoutDeposit.IsChecked == true) UserData.DepositFilter = 2;

            LoadHalls();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _1LoginPage());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var zal = button.DataContext as Halls;
            if (zal == null) return;

            UserData.SelectedHall = zal;
            NavigationService?.Navigate(new _5BookingPage());
        }

        private void BtnRestiki_Click(object sender, RoutedEventArgs e)
        {
            UserData.SelectedRestaurant = null;
            NavigationService?.Navigate(new _3RestoranPage());
        }
    }
}
