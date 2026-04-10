using Sonic.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sonic
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            SetDarkTheme();
        }

        private void ButtonD_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MenuColumn != null)
            {
                if (MenuColumn.Width.Value > 0)
                    MenuColumn.Width = new System.Windows.GridLength(0);
                else
                    MenuColumn.Width = new System.Windows.GridLength(134);
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BClean_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CleanupPage());
        }

        private void hide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void size_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsPage());
        }

   
        public void SetLightTheme()
        {
            this.Background = Brushes.White;
            if (this.Content is Grid mainGrid) mainGrid.Background = Brushes.White;
            if (MainFrame != null) MainFrame.Background = Brushes.White;
            Application.Current.Resources["ThemeTextBrush"] = Brushes.Black;
            Application.Current.Resources["ThemeButtonBrush"] = Brushes.Black;
            Application.Current.Resources["UpThemeButtonBrush"] = Brushes.White;
            Application.Current.Resources["UpThemeTextBrush"] = Brushes.Black;
        }

        public void SetDarkTheme()
        {
            this.Background = Brushes.Black;
            if (this.Content is Grid mainGrid) mainGrid.Background = Brushes.Black;
            if (MainFrame != null) MainFrame.Background = Brushes.Black;
            Application.Current.Resources["ThemeTextBrush"] = Brushes.Red;
            Application.Current.Resources["ThemeButtonBrush"] = Brushes.Red;
            Application.Current.Resources["UpThemeButtonBrush"] = Brushes.Black;
            Application.Current.Resources["UpThemeTextBrush"] = Brushes.Red;
        }
    }
}