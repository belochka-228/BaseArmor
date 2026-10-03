using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для _8ChangePassPage.xaml
    /// </summary>
    public partial class _8ChangePassPage : Page
    {
        public _8ChangePassPage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Хеширование пароля (SHA1)
        /// </summary>
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }
        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtLoginHint.Visibility = string.IsNullOrEmpty(txtLogin.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void txtLoginHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtLogin.Focus();
        }

        private void txtOldPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtOldPasswordHint.Visibility = string.IsNullOrEmpty(txtOldPassword.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void txtOldPasswordHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtOldPassword.Focus();
        }

        private void txtNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtNewPasswordHint.Visibility = string.IsNullOrEmpty(txtNewPassword.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void txtNewPasswordHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtNewPassword.Focus();
        }

        private void txtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtConfirmPasswordHint.Visibility = string.IsNullOrEmpty(txtConfirmPassword.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void txtConfirmPasswordHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtConfirmPassword.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string oldPassword = txtOldPassword.Password;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) ||
                string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Все поля обязательны к заполнению!");
                return;
            }

            string hashedOldPass = GetHash(oldPassword);
            var user = Core.Context.Users.FirstOrDefault(u => u.login == login && u.password_hash == hashedOldPass);

            if (user == null)
            {
                MessageBox.Show("Неправильный логин или пароль!");
                return;
            }

            bool en = true;
            bool number = false;

            if (newPassword.Length >= 6)
            {
                for (int i = 0; i < newPassword.Length; i++)
                {
                    if (newPassword[i] >= '0' && newPassword[i] <= '9') number = true;
                    else if (!((newPassword[i] >= 'A' && newPassword[i] <= 'Z') || (newPassword[i] >= 'a' && newPassword[i] <= 'z'))) en = false;
                }
            }
            else
            {
                MessageBox.Show("Пароль должен содержать 6 или более символов!");
                return;
            }

            if (!en)
            {
                MessageBox.Show("Допускается только английская раскладка!");
                return;
            }

            if (!number)
            {
                MessageBox.Show("Добавьте хотя бы одну цифру!");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            user.password_hash = GetHash(newPassword);
            Core.Context.SaveChanges();

            MessageBox.Show("Пароль успешно изменен!");
            NavigationService?.Navigate(new _1LoginPage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _1LoginPage());
        }
    }
}
