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


namespace BaseArmor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadTheme();
            MainFrame.Navigate(new _3RestoranPage());
        }

        /// <summary>
        /// Загрузка темы из настроек
        /// </summary>
        private void LoadTheme()
        {
            bool isDark = Properties.Settings.Default.IsDarkTheme;
            ApplyTheme(isDark);
        }

        /// <summary>
        /// Применение темы
        /// </summary>
        private void ApplyTheme(bool isDark)
        {
            var uri = new Uri(isDark ? "DictionaryDark.xaml" : "Dictionary.xaml", UriKind.Relative);
            var dict = Application.LoadComponent(uri) as ResourceDictionary;


            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);

            if (ThemeButton != null)
                ThemeButton.Content = isDark ? "☀️ Тема" : "🌙 Тема";
        }
        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            bool isDark = !Properties.Settings.Default.IsDarkTheme;
            Properties.Settings.Default.IsDarkTheme = isDark;
            Properties.Settings.Default.Save(); 

            ApplyTheme(isDark);
            RefreshCurrentPage();
        }

        private void RefreshCurrentPage()
        {
            if (MainFrame.Content is Page currentPage)
            {
                var type = currentPage.GetType();

                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    try
                    {
                        var newPage = Activator.CreateInstance(type) as Page;
                        if (newPage != null)
                        {
                            MainFrame.Navigate(newPage);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserData.CurrentUser == null)
            {
                MessageBox.Show("Вы не авторизованы.");
                MainFrame.Navigate(new _1LoginPage());
                return;
            }

            MainFrame.Navigate(new _6UseProfPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Выйти из системы?", "Выход", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                UserData.CurrentUser = null;
                UserData.SelectedRestaurant = null;
                UserData.SelectedHall = null;
                MainFrame.Navigate(new _1LoginPage());
            }
        }
        private void MainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var page = e.Content as Page;
            string pageName = page.GetType().Name;
            if (page != null)
            {
                string title = "Котики вперед";

                switch (pageName)
                {
                    case "_1LoginPage": title = ""; break;
                    case "_2RegistrPage": title = ""; break;
                    case "_8ChangePassPage": title = ""; break;
                    case "_3RestoranPage": title = "Выбор ресторана"; break;
                    case "_4HallPage": title = "Выбор зала"; break;
                    case "_5BookingPage": title = "Бронирование"; break;
                    case "_6UseProfPage": title = "Личный кабинет"; break;
                    case "_7AdminPage": title = "Панель администратора"; break;
                    case "_9DepozitPay": title = "Оплата депозита"; break;
                    case "UsersTabPage": title = "Таблица пользователей"; break;
                    case "AddUserPage": title = "Редактирование/добавление пользователя"; break;
                    case "RestaurantsTabPage": title = "Таблица ресторанов"; break;
                    case "AddRestaurantPage": title = "Редактирование/добавление ресторана"; break;
                    case "BookingTabPage": title = "Таблица броней"; break;
                    case "AddBookingPage": title = "Редактирование/добавление брони"; break;
                    case "DiagrammPage": title = "Диаграммы"; break;
                }
                PageTitleText.Text = title;
            }
            bool isAuthPage = pageName == "_1LoginPage" || pageName == "_2RegistrPage" || pageName == "_8ChangePassPage";

            ProfileButton.Visibility = isAuthPage ? Visibility.Collapsed : Visibility.Visible;
            LogoutButton.Visibility = isAuthPage ? Visibility.Collapsed : Visibility.Visible;

            if (!isAuthPage)
            {
                bool hideProfile = pageName == "_6UseProfPage" || pageName == "_7AdminPage" || pageName == "UsersTabPage"
                                || pageName == "AddUserPage" || pageName == "RestaurantsTabPage"
                                || pageName == "AddRestaurantPage" || pageName == "BookingTabPage"
                                || pageName == "AddBookingPage" || pageName == "DiagrammPage"
                                || (UserData.CurrentUser != null && UserData.CurrentUser.role_id == 1);
                ProfileButton.Visibility = hideProfile ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Message",
            MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                e.Cancel = true;
            else
            {
                Properties.Settings.Default.Save();
                e.Cancel = false;
            }
        }
    }
}
