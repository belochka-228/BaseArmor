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
    /// Логика взаимодействия для _9DepozitPay.xaml
    /// </summary>
    public partial class _9DepozitPay : Page
    {
        public _9DepozitPay()
        {
            InitializeComponent();
            if (UserData.CurrentDepozit == null)
            {
                MessageBox.Show("Депозит не найден.");
                return;
            }

            txtOplata.Text = $"{UserData.CurrentDepozit.amount:N0} ₽";
        }
        private void PaymentMethod_Checked(object sender, RoutedEventArgs e)
        {
            if (PanelCard == null || PanelQR == null) return;

            if (rbCard.IsChecked == true)
            {
                PanelCard.Visibility = Visibility.Visible;
                PanelQR.Visibility = Visibility.Collapsed;
            }
            else if (rbSBP.IsChecked == true)
            {
                PanelCard.Visibility = Visibility.Collapsed;
                PanelQR.Visibility = Visibility.Visible;
            }
        }
        private void txtCardNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtCardNumberHint.Visibility = string.IsNullOrEmpty(txtCardNumber.Text) ? Visibility.Visible : Visibility.Hidden;
        }
        private void txtCardNumberHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => txtCardNumber.Focus();

        private void txtCardDate_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtCardDateHint.Visibility = string.IsNullOrEmpty(txtCardDate.Text) ? Visibility.Visible : Visibility.Hidden;
        }
        private void txtCardDateHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => txtCardDate.Focus();

        private void txtCVV_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtCVVHint.Visibility = string.IsNullOrEmpty(txtCVV.Text) ? Visibility.Visible : Visibility.Hidden;
        }
        private void txtCVVHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => txtCVV.Focus();
        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            if (rbCard.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(txtCardNumber.Text) ||
                    string.IsNullOrWhiteSpace(txtCardDate.Text) ||
                    string.IsNullOrWhiteSpace(txtCVV.Text))
                {
                    MessageBox.Show("Заполните все поля карты.");
                    return;
                }
            }
            int methodId = rbCard.IsChecked == true ? 1 : 2;

            var deposit = Core.Context.Deposit.FirstOrDefault(d => d.id == UserData.CurrentDepozit.id);
            if (deposit != null)
            {
                deposit.status_id = 2;
                deposit.method_id = methodId;
                deposit.payment_date = DateTime.Now;

                var booking = Core.Context.Booking.FirstOrDefault(b => b.id == deposit.id);
                if (booking != null)
                    booking.status_id = 2;

                Core.Context.SaveChanges();
            }

            MessageBox.Show("Депозит успешно оплачен! Бронирование подтверждено.");
            UserData.CurrentDepozit = null;
            NavigationService?.Navigate(new _3RestoranPage());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var deposit = Core.Context.Deposit.FirstOrDefault(d => d.id == UserData.CurrentDepozit.id);
            if (deposit != null)
                Core.Context.Deposit.Remove(deposit);

            var booking = Core.Context.Booking.FirstOrDefault(b => b.id == UserData.CurrentDepozit.id);
            if (booking != null)
                Core.Context.Booking.Remove(booking);

            Core.Context.SaveChanges();

            MessageBox.Show("Бронирование отменено.");
            UserData.CurrentDepozit = null;
            NavigationService?.Navigate(new _3RestoranPage());
        }
    }
}
