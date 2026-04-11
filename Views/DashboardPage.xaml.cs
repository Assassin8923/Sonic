using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;

namespace Sonic
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Оновлення даних...";
            await Task.Delay(2000);
            TxtTempSize.Text = "1.5 GB";
            StatusText.Text = "Дані оновлено";
        }
    }
}
