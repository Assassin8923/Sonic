using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Sonic.Views
{
    public partial class CustomMessage : Window
    {
        public CustomMessage()
        {
            InitializeComponent();
        }

        public CustomMessage(string message, string title)
        {
            InitializeComponent();
            MessageText.Text = message;
            TitleText.Text = title;
        }

        private void Grid_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                this.BeginMoveDrag(e);
            }
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void ShowMessage(string message, string title = "Sonic")
        {
            var msgBox = new CustomMessage(message, title);
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
            {
                msgBox.ShowDialog(desktop.MainWindow);
            }
            else
            {
                msgBox.Show();
            }
        }
    }
}
