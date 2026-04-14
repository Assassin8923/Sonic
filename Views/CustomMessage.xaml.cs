using System;
using System.Windows;
using System.Windows.Input;

namespace Sonic.Views
{
    public partial class CustomMessage : Window
    {
        public CustomMessage(string message, string title)
        {
            InitializeComponent();
            MessageText.Text = message;
            TitleText.Text = title;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void ShowMessage(string message, string title = "Sonic")
        {
            CustomMessage msgBox = new CustomMessage(message, title);
            msgBox.ShowDialog();
        }
    }
}