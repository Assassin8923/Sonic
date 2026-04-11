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
            this.SetResourceReference(Window.BackgroundProperty, "ThemeBackgroundBrush");
            if (Properties.Settings.Default.IsDarkTheme)
                SetDarkTheme();
            else
                SetLightTheme();
            this.Width = Properties.Settings.Default.WindowWidth;
            this.Height = Properties.Settings.Default.WindowHeight;
        }

        private void ButtonD_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MenuColumn.Width.Value > 0)
            {
                MenuColumn.Width = new GridLength(0);
            }
            else
            {
                MenuColumn.Width = new GridLength(140);
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
            var dict = new ResourceDictionary() { Source = new Uri("Resources/LightTheme.xaml", UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries[0] = dict;
            Properties.Settings.Default.IsDarkTheme = false ;
        }   

        public void SetDarkTheme()
        {
            var dict = new ResourceDictionary() {Source = new Uri("Resources/DarkTheme.xaml", UriKind.Relative) };
            Application.Current.Resources.MergedDictionaries[0] = dict;
            Properties.Settings.Default.IsDarkTheme = true;
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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Properties.Settings.Default.WindowWidth = this.RestoreBounds.Width;
                Properties.Settings.Default.WindowHeight = this.RestoreBounds.Height;
            }
            else
            {
                Properties.Settings.Default.WindowWidth = this.Width;
                Properties.Settings.Default.WindowHeight = this.Height;
            }
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        }
    }
}