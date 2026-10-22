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
