using Avalonia;
using Avalonia.Controls;
using Sonic.Properties;
using Sonic.ViewModels;

namespace Sonic.Views
{
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
            if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (desktop.MainWindow is MainWindow mainWindow)
                {
                    DataContext = new SettingsViewModel(
                        Settings.IsDarkTheme,
                        isDarkTheme =>
                        {
                            if (isDarkTheme)
                            {
                                mainWindow.SetDarkTheme();
                            }
                            else
                            {
                                mainWindow.SetLightTheme();
                            }
                        });
                }
            }
        }
    }
}
