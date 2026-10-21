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
using BaseArmor.CRUD;

namespace BaseArmor.Pages
{
    /// <summary>
    /// Логика взаимодействия для _7AdminPage.xaml
    /// </summary>
    public partial class _7AdminPage : Page
    {
        public _7AdminPage()
        {
            InitializeComponent();
        }
        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new UsersTabPage());
        }

        private void BtnRestaurants_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RestaurantsTabPage());
        }

        private void BtnBooking_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new BookingTabPage());
        }

        private void BtnDiagram_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new DiagrammPage());
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
