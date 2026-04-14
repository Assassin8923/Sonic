using System;
using System.IO;
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
using System.Threading;
using System.Threading.Tasks;

namespace Sonic
{
    public partial class DashboardPage : Page
    {
        private CancellationTokenSource? _cts;

        public DashboardPage()
        {
            InitializeComponent();
            _ = UpdateTempSizeAsync();
        }

        private async Task UpdateTempSizeAsync()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                string userTempPath = System.IO.Path.GetTempPath();
                string systemTempPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");

                StatusText.Text = $"Обчислення розміру {userTempPath} і {systemTempPath} ...";
                TxtTempSize.Text = "Обчислення...";

                long userTempSize = await Task.Run(() => GetDirectorySizeSafe(userTempPath, token), token);
                long systemTempSize = await Task.Run(() => GetDirectorySizeSafe(systemTempPath, token), token);
                long totalTempSize = checked(userTempSize + systemTempSize);

                TxtTempSize.Text = FormatBytes(totalTempSize);
                StatusText.Text = $"Готово.";
            }
            catch (OperationCanceledException)
            {
                StatusText.Text = "Операцію скасовано.";
                TxtTempSize.Text = "Н/Д";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Помилка під час обчислення: {ex.Message}";
                TxtTempSize.Text = "Н/Д";
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        private long GetDirectorySizeSafe(string path, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            long total = 0;

            string[] files;
            try
            {
                files = Directory.GetFiles(path);
            }
            catch
            {
                return 0;
            }

            foreach (var file in files)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    var fi = new FileInfo(file);
                    total = checked(total + fi.Length);
                }
                catch
                {
                }
            }

            string[] dirs;
            try
            {
                dirs = Directory.GetDirectories(path);
            }
            catch
            {
                return total;
            }

            foreach (var dir in dirs)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    total = checked(total + GetDirectorySizeSafe(dir, token));
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                }
            }

            return total;
        }

        public string FormatBytes(long bytes)
        {
            if (bytes <= 0) return "0 Б";
            string[] sizes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
