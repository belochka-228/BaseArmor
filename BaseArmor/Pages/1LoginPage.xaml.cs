using Microsoft.Win32;
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
    /// Логика взаимодействия для _1LoginPage.xaml
    /// </summary>
    public partial class _1LoginPage : Page
    {
        public _1LoginPage()
        {
            InitializeComponent();
        }
        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            string login = txtUser.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните логин и пароль.");
                return;
            }

            var user = Core.Context.Users.Include("Roles").FirstOrDefault(u => u.login == login && u.password_hash == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.");
                return;
            }
            UserData.CurrentUser = user;

            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new _3RestoranPage());
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _2RegistrPage());
        }
    }
}

