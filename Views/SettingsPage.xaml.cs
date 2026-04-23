using System.Windows;
using System.Windows.Controls;
using Sonic.ViewModels;

namespace Sonic.Views
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                DataContext = new SettingsViewModel(
                    Properties.Settings.Default.IsDarkTheme,
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
