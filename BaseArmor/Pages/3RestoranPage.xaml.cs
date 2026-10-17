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
using BaseArmor.Pages;

namespace BaseArmor.Pages
{
    /// <summary>
    /// Логика взаимодействия для _3RestoranPage.xaml
    /// </summary>
    public partial class _3RestoranPage : Page
    {
        public _3RestoranPage()
        {
            InitializeComponent();
            cmbSort.SelectedIndex = UserData.RestoranSort;
            LoadRestorans();
        }
        private void LoadRestorans()
        {
            var restoran = Core.Context.Restaurants.AsQueryable();

            string source = txtSource.Text.ToLower().Trim();
            if(!string.IsNullOrWhiteSpace(source)) restoran = restoran.Where(r => r.name.ToLower().Contains(source) || r.address.ToLower().Contains(source ));
        
            switch(cmbSort.SelectedIndex)
            {
                case 0:
                    restoran = restoran.OrderByDescending(r => r.rating);
                    break;
                case 1:
                    restoran = restoran.OrderBy(r => r.average_check);
                    break;
                case 2:
                    restoran = restoran.OrderByDescending(r => r.average_check); 
                    break;
            }
            ListRestorans.ItemsSource = restoran.ToList();
        }
        private void txtSource_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadRestorans();
        }
        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserData.RestoranSort = cmbSort.SelectedIndex;
            LoadRestorans();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var rest = button.DataContext as Restaurants;
            if(rest == null) return;

            UserData.SelectedRestaurant = rest;
            NavigationService.Navigate(new _4HallPage());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _1LoginPage());
        }

        private void BtnProfil_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _6UseProfPage());
        }
    }
}
