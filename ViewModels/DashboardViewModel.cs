using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sonic.ViewModels
{
    public class DashboardViewModel : BindableBase
    {
        private string _tempSize = "Обчислення...";
        private string _statusText = "Статус обчислення.";
        private bool _isRefreshing;
        private CancellationTokenSource? _cts;

        public string TempSize
        {
            get => _tempSize;
            set => SetProperty(ref _tempSize, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public async Task RefreshAsync()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                string userTempPath = Path.GetTempPath();
                string systemTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");

                IsRefreshing = true;
                StatusText = $"Обчислення розміру {userTempPath} і {systemTempPath} ...";
                TempSize = "Обчислення...";

                long userTempSize = await Task.Run(() => GetDirectorySizeSafe(userTempPath, token), token);
                long systemTempSize = await Task.Run(() => GetDirectorySizeSafe(systemTempPath, token), token);
                long totalTempSize = checked(userTempSize + systemTempSize);

                TempSize = FormatBytes(totalTempSize);
                StatusText = "Готово.";
            }
            catch (OperationCanceledException)
            {
                StatusText = "Операцію скасовано.";
                TempSize = "Н/Д";
            }
            catch (Exception ex)
            {
                StatusText = $"Помилка під час обчислення: {ex.Message}";
                TempSize = "Н/Д";
            }
            finally
            {
                IsRefreshing = false;
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
                    var fileInfo = new FileInfo(file);
                    total = checked(total + fileInfo.Length);
                }
                catch
                {
                }
            }

            string[] directories;
            try
            {
                directories = Directory.GetDirectories(path);
            }
            catch
            {
                return total;
            }

            foreach (var directory in directories)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    total = checked(total + GetDirectorySizeSafe(directory, token));
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

        private string FormatBytes(long bytes)
        {
            if (bytes <= 0)
            {
                return "0 Б";
            }

            string[] sizes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
            int order = 0;
            double length = bytes;

            while (length >= 1024 && order < sizes.Length - 1)
            {
                order++;
                length /= 1024;
            }

            return $"{length:0.##} {sizes[order]}";
        }
    }
}
