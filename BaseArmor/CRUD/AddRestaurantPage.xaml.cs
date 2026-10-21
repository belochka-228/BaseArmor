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
    /// Логика взаимодействия для AddRestaurantPage.xaml
    /// </summary>
    public partial class AddRestaurantPage : Page
    {
        private Restaurants _currentRestaurant;
        private bool _isEdit = false;

        public AddRestaurantPage(Restaurants restaurant)
        {
            InitializeComponent();

            _isEdit = restaurant != null;
            _currentRestaurant = restaurant ?? new Restaurants();

            TitleText.Text = _isEdit ? "Редактирование ресторана" : "Добавление ресторана";

            DataContext = _currentRestaurant;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentRestaurant.name))
                errors.AppendLine("Укажите название.");
            if (string.IsNullOrWhiteSpace(_currentRestaurant.address))
                errors.AppendLine("Укажите адрес.");
            if (_currentRestaurant.rating == null || _currentRestaurant.rating < 0 || _currentRestaurant.rating > 5)
                errors.AppendLine("Рейтинг должен быть от 0 до 5.");
            if (_currentRestaurant.average_check == null || _currentRestaurant.average_check < 0)
                errors.AppendLine("Средний чек должен быть положительным.");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка");
                return;
            }

            try
            {
                if (!_isEdit)
                    Core.Context.Restaurants.Add(_currentRestaurant);

                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.Navigate(new RestaurantsTabPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RestaurantsTabPage());
        }
    }
}