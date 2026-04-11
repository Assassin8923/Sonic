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

namespace Sonic.Views
{
    public partial class SettingsPage : Page
    {
         public SettingsPage()
        {
            InitializeComponent();
            var mainWindow = Application.Current.MainWindow;
            if (Properties.Settings.Default.IsDarkTheme)
            {
                Nslider.Value = 0;
                Nlable.Content = "Темна";
            }
            else
            {
                Nslider.Value = 1;
                Nlable.Content = "Світла";
            }
        }

        private void Nslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Application.Current.MainWindow is MainWindow myWindow)
            {
                if (Nslider.Value == 1)
                {
                    myWindow.SetLightTheme();
                    Nlable.Content = "Світла";
                }
                else if (Nslider.Value == 0)
                {
                    myWindow.SetDarkTheme();
                    Nlable.Content = "Темна";
                }
            }
        }
    }
}
