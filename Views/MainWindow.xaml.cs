using System.Windows;

namespace Sonic
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // При запуску одразу відкриваємо сторінку діагностики
            MainFrame.Navigate(new DashboardPage());
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void BtnCleaner_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CleanerPage());
        }

        private void ButtonD_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}