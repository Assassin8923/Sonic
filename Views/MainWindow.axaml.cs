using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using Sonic.Properties;
using System;

namespace Sonic
{
    public partial class MainWindow : Window
    {
        private const double ExpandedMenuWidth = 140;
        private bool fullscreen = false;
        private ColumnDefinition? _menuColumn;

        public MainWindow()
        {
            InitializeComponent();
            _menuColumn = ((Grid)this.Content!).ColumnDefinitions![0];

            this.Background = (IBrush)this.FindResource("ThemeBackgroundBrush")!;

            if (Settings.IsDarkTheme)
                SetDarkTheme();
            else
                SetLightTheme();

            this.Width = Settings.WindowWidth;
            this.Height = Settings.WindowHeight;

            MainFrame?.Content = new DashboardPage();
        }

        private void ButtonD_Click(object? sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DashboardPage();
        }

        private void ToggleMenu_Click(object? sender, RoutedEventArgs e)
        {
            if (_menuColumn!.Width.Value > 0)
            {
                _menuColumn.Width = new GridLength(0);
                MenuViewbox?.IsVisible = false;
            }
            else
            {
                _menuColumn.Width = new GridLength(ExpandedMenuWidth);
                MenuViewbox?.IsVisible = true;
            }
        }

        private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                this.BeginMoveDrag(e);
            }
        }

        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BClean_Click(object? sender, RoutedEventArgs e)
        {
            MainFrame?.Content = new Views.CleanupPage();
        }

        private void hide_Click(object? sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void size_Click(object? sender, RoutedEventArgs e)
        {
            SizeButton?.Content = this.WindowState == WindowState.Maximized ? "🗖" : "🗗";
            if (this.WindowState == WindowState.Normal)
            {
                this.MaxHeight = Screens.Primary.WorkingArea.Height;
                this.MaxWidth = Screens.Primary.WorkingArea.Width;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void Button_Click_1(object? sender, RoutedEventArgs e)
        {
            MainFrame?.Content = new Views.SettingsPage();
        }

        public void SetLightTheme()
        {
            var dict = AvaloniaXamlLoader.Load(new Uri("avares://Sonic/Resources/LightTheme.axaml")) as ResourceDictionary;
            if (dict != null)
            {
                this.Resources.MergedDictionaries.Clear();
                this.Resources.MergedDictionaries.Add(dict);
            }
            Settings.IsDarkTheme = false;
        }

        public void SetDarkTheme()
        {
            var dict = AvaloniaXamlLoader.Load(new Uri("avares://Sonic/Resources/DarkTheme.axaml")) as ResourceDictionary;
            if (dict != null)
            {
                this.Resources.MergedDictionaries.Clear();
                this.Resources.MergedDictionaries.Add(dict);
            }
            Settings.IsDarkTheme = true;
        }

        private void Window_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                if (fullscreen)
                {
                    this.MaxHeight = Screens.Primary.WorkingArea.Height;
                    this.MaxWidth = Screens.Primary.WorkingArea.Width;
                    this.WindowState = WindowState.Normal;
                    fullscreen = false;
                    SizeButton.IsVisible = true;
                    hide.IsVisible = true;
                    ButtonCl.IsVisible = true;
                    exitbutton?.IsVisible = false;
                }
                else
                {
                    this.MaxHeight = double.PositiveInfinity;
                    this.MaxWidth = double.PositiveInfinity;
                    this.WindowState = WindowState.Maximized;
                    fullscreen = true;
                    SizeButton.IsVisible = false;
                    hide.IsVisible = false;
                    ButtonCl.IsVisible = false;
                    exitbutton?.IsVisible = true;
                }
                e.Handled = true;
            }
        }

        public void Exit_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Properties.Settings.WindowWidth = this.Width;
                Properties.Settings.WindowHeight = this.Height;
            }
            else
            {
                Properties.Settings.WindowWidth = this.Width;
                Properties.Settings.WindowHeight = this.Height;
            }
            Properties.Settings.Save();
            base.OnClosing(e);
        }
    }
}
