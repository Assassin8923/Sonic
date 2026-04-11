using Sonic.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sonic
{
    public partial class MainWindow : Window
    {
        private bool fullscreen = false;

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
                if (MenuColumn.Width.Value > 0)
                    MenuColumn.Width = new System.Windows.GridLength(0);
                else
                    MenuColumn.Width = new System.Windows.GridLength(160);
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
            SizeButton.Content = this.WindowState == WindowState.Maximized ? "🗖" : "🗗";
            if (this.WindowState == WindowState.Normal)
            {
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
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
            Application.Current.Resources["ThemeButtonTextBrush"] = Brushes.White;
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
            Application.Current.Resources["ThemeButtonTextBrush"] = Brushes.Black;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                if (fullscreen)
                {
                     this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
                     this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
                    this.WindowState = WindowState.Normal;
                    fullscreen = false;
                    SizeButton.Visibility = fullscreen ? Visibility.Hidden : Visibility.Visible;
                    hide.Visibility = fullscreen ? Visibility.Hidden : Visibility.Visible;
                    ButtonCl.Visibility = fullscreen ? Visibility.Hidden : Visibility.Visible;
                    exitbutton.Visibility = fullscreen ? Visibility.Visible : Visibility.Hidden;
                }
                else
                {
                     this.MaxHeight = double.PositiveInfinity;
                     this.MaxWidth = double.PositiveInfinity;
                    this.WindowState = WindowState.Maximized;
                    fullscreen = true;
                    SizeButton.Visibility = fullscreen ? Visibility.Hidden : Visibility.Visible;
                    hide.Visibility = fullscreen ? Visibility.Hidden : Visibility.Visible;
                    ButtonCl.Visibility = fullscreen ? Visibility.Hidden : Visibility.Visible;
                    exitbutton.Visibility = fullscreen ? Visibility.Visible : Visibility.Hidden;
                }
                e.Handled = true;
            }
        }

        public void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}