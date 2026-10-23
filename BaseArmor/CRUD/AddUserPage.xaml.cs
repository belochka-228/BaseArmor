using Microsoft.Office.Interop.Word;
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
    /// Логика взаимодействия для AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : System.Windows.Controls.Page
    {
        private Users _currentUser;
        private bool _isEdit = false;

        public AddUserPage(Users user)
        {
            InitializeComponent();

            _isEdit = user != null;
            _currentUser = user ?? new Users();
            cmbRole.ItemsSource = Core.Context.Roles.ToList();
            DataContext = _currentUser;
            if (!_isEdit)
            {
                cmbRole.SelectedIndex = 1;
                chkActive.IsChecked = true;
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentUser.last_name))
                errors.AppendLine("Укажите фамилию.");
            if (string.IsNullOrWhiteSpace(_currentUser.first_name))
                errors.AppendLine("Укажите имя.");
            if (string.IsNullOrWhiteSpace(_currentUser.login))
                errors.AppendLine("Укажите логин.");
            if (string.IsNullOrWhiteSpace(_currentUser.phone))
                errors.AppendLine("Укажите телефон.");
            if (string.IsNullOrWhiteSpace(_currentUser.email))
                errors.AppendLine("Укажите email.");
            if (cmbRole.SelectedItem == null)
                errors.AppendLine("Выберите роль.");

            var existing = Core.Context.Users
                .FirstOrDefault(u => u.login == _currentUser.login && u.id != _currentUser.id);
            if (existing != null)
                errors.AppendLine("Пользователь с таким логином уже существует.");
            if (!IsValidEmail(_currentUser.email))
            {
                MessageBox.Show("Введите корректный email (пример: user@mail.ru).");
                return;
            }
            if (Core.Context.Users.Any(u => u.email == _currentUser.email && u.id != _currentUser.id))
            {
                MessageBox.Show("Пользователь с такой почтой уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Core.Context.Users.Any(u => u.phone == _currentUser.phone && u.id != _currentUser.id))
            {
                MessageBox.Show("Пользователь с таким номером телефона уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка");
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                _currentUser.password_hash = GetHash(txtPassword.Password);
            }
            else if (!_isEdit)
            {
                MessageBox.Show("Для нового пользователя введите пароль.");
                return;
            }

            _currentUser.role_id = (int)cmbRole.SelectedValue;
            _currentUser.is_active = chkActive.IsChecked == true;

            try
            {
                if (!_isEdit)
                    Core.Context.Users.Add(_currentUser);

                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.Navigate(new UsersTabPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new UsersTabPage());
        }
        public static string GetHash(string password)
        {
            using (var hash = System.Security.Cryptography.SHA1.Create())
            {
                return string.Concat(hash
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
                    .Select(x => x.ToString("X2")));
            }
        }
    }
}
