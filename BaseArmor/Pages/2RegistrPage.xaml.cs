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
    /// Логика взаимодействия для _2RegistrPage.xaml
    /// </summary>
    public partial class _2RegistrPage : Page
    {
        public _2RegistrPage()
        {
            InitializeComponent();
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string number = txtNumber.Text.Trim();
            string password = txtPassword.Password;
            string passwordConfirm = txtPasswordConfirm.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(firstName) ||string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(number) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(passwordConfirm))
            {
                MessageBox.Show("Все поля обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (password != passwordConfirm)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Core.Context.Users.Any(u => u.login == login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Core.Context.Users.Any(u => u.phone == number))
            {
                MessageBox.Show("Пользователь с таким email уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var newUser = new Users
            {
                login = login,
                password_hash = password,
                first_name = firstName,
                last_name = lastName,
                phone = number,
                role_id = 2,
                is_active = true
            };
            Core.Context.Users.Add(newUser);
            Core.Context.SaveChanges();
            MessageBox.Show("Регистрация прошла успешно! Теперь войдите.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService?.Navigate(new _1LoginPage());
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _1LoginPage());
        }

    }
}
