using Avalonia.Controls;
using Avalonia.Interactivity;
using Sonic.ViewModels;

namespace Sonic.Views
{
    public partial class CleanupPage : UserControl
    {
        private readonly CleanupViewModel _viewModel;

        public CleanupPage()
        {
            InitializeComponent();
            _viewModel = new CleanupViewModel();
            DataContext = _viewModel;
        }

        private async void CButton_Click(object? sender, RoutedEventArgs e)
        {
            CleanupResult result = await _viewModel.RunCleanupAsync();
            if (result.IsSuccess)
            {
                CustomMessage.ShowMessage(result.Message, "Sonic");
            }
            else
            {
                CustomMessage.ShowMessage(result.Message, "Помилка");
            }
        }
    }
}
