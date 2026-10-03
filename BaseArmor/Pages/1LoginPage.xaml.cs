using Microsoft.Win32;
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
    /// Логика взаимодействия для _1LoginPage.xaml
    /// </summary>
    public partial class _1LoginPage : Page
    {
        private int failedAttempts = 0;
        public _1LoginPage()
        {
            InitializeComponent();
            failedAttempts = UserData.FailedAttempts;
            if (UserData.CaptchaVisible)
            {
                CaptchaSwitch();
                CaptchaChange();
            }
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
        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            string login = txtUser.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните логин и пароль.");
                return;
            }
            string hashedPassword = GetHash(password);

            var user = Core.Context.Users.Include("Roles").FirstOrDefault(u => u.login == login && u.password_hash == hashedPassword);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.");
                failedAttempts++;
                UserData.FailedAttempts = failedAttempts;

                if (failedAttempts >= 3)
                {
                    if (captcha.Visibility != Visibility.Visible)
                    {
                        CaptchaSwitch();
                    }
                    CaptchaChange();
                }
                return;
            }
            failedAttempts = 0;
            UserData.FailedAttempts = 0;
            UserData.CaptchaVisible = false;
            UserData.CurrentUser = user;

            switch (user.Roles.name)
            {
                case "Администратор":
                    NavigationService?.Navigate(new _7AdminPage());
                    break;
                case "Клиент":
                    NavigationService?.Navigate(new _3RestoranPage());
                    break;
                default:
                    NavigationService?.Navigate(new _3RestoranPage());
                    break;
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _2RegistrPage());
        }

        private void txtUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtUserHint.Visibility = string.IsNullOrEmpty(txtUser.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void txtUserHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtUser.Focus();
        }

        private void txtPasswordHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtPassword.Focus();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtPasswordHint.Visibility = string.IsNullOrEmpty(txtPassword.Password) ? Visibility.Visible : Visibility.Hidden;
        }
        public void CaptchaSwitch()
        {
            switch (captcha.Visibility)
            {
                case Visibility.Visible:
                    txtUser.Clear();
                    txtPassword.Clear();

                    captcha.Visibility = Visibility.Hidden;
                    captchaInput.Visibility = Visibility.Hidden;
                    txtCaptcha.Visibility = Visibility.Hidden;
                    submitCaptcha.Visibility = Visibility.Hidden;

                    txtUser.Visibility = Visibility.Visible;
                    txtPassword.Visibility = Visibility.Visible;
                    txtUser.Visibility = Visibility.Visible;
                    txtUserHint.Visibility = Visibility.Visible;
                    txtPassword.Visibility = Visibility.Visible;
                    txtPasswordHint.Visibility = Visibility.Visible;

                    ButtonChangePassword.Visibility = Visibility.Visible;
                    UserData.CaptchaVisible = false;
                    return;

                case Visibility.Hidden:
                    captcha.Visibility = Visibility.Visible;
                    captchaInput.Visibility = Visibility.Visible;
                    txtCaptcha.Visibility = Visibility.Visible;
                    submitCaptcha.Visibility = Visibility.Visible;

                    txtUser.Visibility = Visibility.Hidden;
                    txtPassword.Visibility = Visibility.Hidden;
                    txtUser.Visibility = Visibility.Hidden;
                    txtUserHint.Visibility = Visibility.Hidden;
                    txtPassword.Visibility = Visibility.Hidden;
                    txtPasswordHint.Visibility = Visibility.Hidden;

                    ButtonChangePassword.Visibility = Visibility.Hidden;
                    UserData.CaptchaVisible = true;
                    return;
            }
        }

        public void CaptchaChange()
        {
            String allowchar = " ";
            allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z";
            allowchar += "1,2,3,4,5,6,7,8,9,0";
            char[] a = { ',' };
            String[] ar = allowchar.Split(a);
            String pwd = "";
            string temp = "";
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                temp = ar[(r.Next(0, ar.Length))];
                pwd += temp;
            }
            captcha.Text = pwd;
        }

        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                MessageBox.Show("Неверно введена капча", "Ошибка");
                captchaInput.Clear();
                CaptchaChange();
            }
            else
            {
                MessageBox.Show("Капча введена успешно, можете продолжить авторизацию", "Успех");
                CaptchaSwitch();
                captchaInput.Clear();
                failedAttempts = 0;
            }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _8ChangePassPage());
        }
    }
}

