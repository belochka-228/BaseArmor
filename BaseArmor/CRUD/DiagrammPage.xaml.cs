using BaseArmor.Pages;
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
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ChartArea = System.Windows.Forms.DataVisualization.Charting.ChartArea;
using Series = System.Windows.Forms.DataVisualization.Charting.Series;
using SeriesChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType;

using Page = System.Windows.Controls.Page;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace BaseArmor.CRUD
{
    /// <summary>
    /// Логика взаимодействия для DiagrammPage.xaml
    /// </summary>
    public partial class DiagrammPage : Page
    {
        public DiagrammPage()
        {
            InitializeComponent();
            CmbUser.ItemsSource = Core.Context.Users.ToList();
            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));

            CmbUser.SelectedIndex = 0;
            CmbDiagram.SelectedItem = SeriesChartType.Column;
        }

        /// <summary>
        /// Обновляет диаграмму при смене пользователя или типа
        /// </summary>
        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (!(CmbUser.SelectedItem is Users currentUser) || !(CmbDiagram.SelectedItem is SeriesChartType currentType))
                return;

            ChartBookings.Series.Clear();
            ChartBookings.ChartAreas.Clear();

            var chartArea = new ChartArea("MainArea");
            ChartBookings.ChartAreas.Add(chartArea);

            var series = new Series("Брони")
            {
                ChartType = currentType,
                ChartArea = "MainArea"
            };

            var bookingsByRestaurant = Core.Context.Booking.Where(b => b.user_id == currentUser.id).ToList()
                .GroupBy(b => b.DiningTable.Halls.Restaurants.name).Select(g => new { Restaurant = g.Key, Count = g.Count() }).ToList();

            foreach (var item in bookingsByRestaurant)
            {
                series.Points.AddXY(item.Restaurant, item.Count);
            }
            ChartBookings.Series.Add(series);
            ChartBookings.Titles.Clear();
            ChartBookings.Titles.Add($"Брони пользователя {currentUser.login}");
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allUsers = Core.Context.Users.ToList().OrderBy(u => u.login).ToList();

                var application = new Excel.Application();
                application.SheetsInNewWorkbook = allUsers.Count;
                Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);

                for (int i = 0; i < allUsers.Count; i++)
                {
                    var currentUser = allUsers[i];
                    int startRowIndex = 1;
                    Excel.Worksheet worksheet = application.Worksheets.Item[i + 1];
                    worksheet.Name = currentUser.login.Length > 31 ? currentUser.login.Substring(0, 31) : currentUser.login;

                    worksheet.Cells[1][startRowIndex] = "Ресторан";
                    worksheet.Cells[2][startRowIndex] = "Количество броней";

                    Excel.Range columnHeaderRange = worksheet.Range[ worksheet.Cells[1][startRowIndex], worksheet.Cells[2][startRowIndex]];
                    columnHeaderRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    columnHeaderRange.Font.Bold = true;
                    startRowIndex++;

                    var bookingsByRestaurant = Core.Context.Booking.Where(b => b.user_id == currentUser.id).ToList()
                        .GroupBy(b => b.DiningTable.Halls.Restaurants.name).Select(g => new { Restaurant = g.Key, Count = g.Count() }).ToList();

                    foreach (var item in bookingsByRestaurant)
                    {
                        worksheet.Cells[1][startRowIndex] = item.Restaurant;
                        worksheet.Cells[2][startRowIndex] = item.Count;
                        startRowIndex++;
                    }
                    worksheet.Columns.AutoFit();
                }

                application.Visible = true;
                MessageBox.Show("Экспорт в Excel завершён!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в Excel: {ex.Message}");
            }
        }
        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var application = new Word.Application();
                Word.Document document = application.Documents.Add();

                var allUsers = Core.Context.Users.ToList().OrderBy(u => u.login).ToList();

                foreach (var user in allUsers)
                {
                    Word.Paragraph userParagraph = document.Paragraphs.Add();
                    Word.Range userRange = userParagraph.Range;
                    userRange.Text = $"Пользователь: {user.last_name} {user.first_name} ({user.login})";
                    userParagraph.set_Style("Заголовок 1");
                    userRange.InsertParagraphAfter();

                    var bookingsByRestaurant = Core.Context.Booking.Where(b => b.user_id == user.id).ToList()
                        .GroupBy(b => b.DiningTable.Halls.Restaurants.name).Select(g => new { Restaurant = g.Key, Count = g.Count() }).ToList();

                    if (bookingsByRestaurant.Count == 0)
                    {
                        Word.Paragraph emptyParagraph = document.Paragraphs.Add();
                        emptyParagraph.Range.Text = "Нет броней.";
                        emptyParagraph.Range.InsertParagraphAfter();
                        continue;
                    }

                    Word.Table table = document.Tables.Add( document.Paragraphs[document.Paragraphs.Count].Range, bookingsByRestaurant.Count + 1, 2);
                    table.Borders.Enable = 1;

                    table.Cell(1, 1).Range.Text = "Ресторан";
                    table.Cell(1, 2).Range.Text = "Количество броней";
                    table.Rows[1].Range.Font.Bold = 1;
                    table.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    for (int i = 0; i < bookingsByRestaurant.Count; i++)
                    {
                        var item = bookingsByRestaurant[i];
                        table.Cell(i + 2, 1).Range.Text = item.Restaurant;
                        table.Cell(i + 2, 2).Range.Text = item.Count.ToString();
                    }
                    document.Paragraphs.Add();
                }

                application.Visible = true;
                MessageBox.Show("Экспорт в Word завершён!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в Word: {ex.Message}");
            }
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new _7AdminPage());
        }
    }
}
