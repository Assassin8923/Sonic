using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Sonic.Models;

namespace Sonic.ViewModels
{
    public class CleanupViewModel : BindableBase
    {
        private string _statusText = "Оберіть варіанти очищення.";
        private bool _isCleaning;

        public CleanupViewModel()
        {
            Options = new ObservableCollection<CleanupOption>
            {
                new CleanupOption("Тимчасові файли", "all-temp"),
                new CleanupOption("Кошик", string.Empty, CleanupTargetKind.RecycleBin),
                new CleanupOption("Кеш браузера (скоро)", string.Empty, CleanupTargetKind.DirectoryGroup, false)
            };

            foreach (var option in Options)
            {
                option.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(CleanupOption.IsSelected))
                    {
                        OnPropertyChanged(nameof(HasSelectedItems));
                        OnPropertyChanged(nameof(CanClean));
                        OnPropertyChanged(nameof(SelectedItemsText));
                    }
                };
            }
        }

        public ObservableCollection<CleanupOption> Options { get; }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public bool IsCleaning
        {
            get => _isCleaning;
            set
            {
                if (SetProperty(ref _isCleaning, value))
                {
                    OnPropertyChanged(nameof(CanClean));
                }
            }
        }

        public bool HasSelectedItems => Options.Any(option => option.IsSelected && option.IsAvailable);

        public bool CanClean => HasSelectedItems && !IsCleaning;

        public string SelectedItemsText
        {
            get
            {
                var selected = Options
                    .Where(option => option.IsSelected)
                    .Select(option => option.Title)
                    .ToArray();

                return selected.Length == 0 ? "Нічого не вибрано" : string.Join(", ", selected);
            }
        }

        public async Task<CleanupResult> RunCleanupAsync()
        {
            IsCleaning = true;
            StatusText = "Виконується очищення...";

            try
            {
                StringBuilder logBuilder = new();
                logBuilder.AppendLine("=========================================");
                logBuilder.AppendLine("ЗВІТ ПРО ОЧИЩЕННЯ SONIC");
                logBuilder.AppendLine($"Дата: {DateTime.Now}");
                logBuilder.AppendLine("=========================================");
                logBuilder.AppendLine();

                long totalFreedBytes = 0;
                bool recycleBinCleaned = false;

                foreach (var option in Options.Where(option => option.IsSelected && option.IsAvailable))
                {
                    if (option.TargetKind == CleanupTargetKind.RecycleBin)
                    {
                        await Task.Run(() => EmptyRecycleBin(logBuilder));
                        recycleBinCleaned = true;
                        continue;
                    }

                    foreach (var directory in GetDirectoriesForOption(option))
                    {
                        totalFreedBytes += await Task.Run(() => CleanDirectory(directory, logBuilder));
                    }
                }

                SaveLogFile(logBuilder.ToString(), totalFreedBytes, recycleBinCleaned);

                var summaryParts = new List<string>();
                if (totalFreedBytes > 0)
                {
                    summaryParts.Add($"звільнено {FormatBytes(totalFreedBytes)}");
                }

                if (recycleBinCleaned)
                {
                    summaryParts.Add("кошик очищено");
                }

                string summaryText = summaryParts.Count == 0
                    ? "Очищення завершено."
                    : $"Очищення завершено: {string.Join(", ", summaryParts)}.";

                StatusText = summaryText;
                return new CleanupResult(true, summaryText);
            }
            catch (Exception ex)
            {
                StatusText = $"Помилка очищення: {ex.Message}";
                return new CleanupResult(false, $"Виникла критична помилка: {ex.Message}");
            }
            finally
            {
                IsCleaning = false;
            }
        }

        private IEnumerable<string> GetDirectoriesForOption(CleanupOption option)
        {
            if (option.Title == "Тимчасові файли")
            {
                return GetAllTempDirectories();
            }

            return string.IsNullOrWhiteSpace(option.Path)
                ? Enumerable.Empty<string>()
                : new[] { option.Path };
        }

        private IEnumerable<string> GetAllTempDirectories()
        {
            HashSet<string> paths = new(StringComparer.OrdinalIgnoreCase);

            void AddIfExists(string path)
            {
                if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
                {
                    paths.Add(path);
                }
            }

            AddIfExists(Path.GetTempPath());
            AddIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"));

            string currentUserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string? usersRoot = Directory.GetParent(currentUserProfile)?.FullName;

            if (!string.IsNullOrWhiteSpace(usersRoot) && Directory.Exists(usersRoot))
            {
                foreach (string userDirectory in Directory.GetDirectories(usersRoot))
                {
                    AddIfExists(Path.Combine(userDirectory, "AppData", "Local", "Temp"));
                }
            }

            return paths;
        }

        private void EmptyRecycleBin(StringBuilder log)
        {
            log.AppendLine("[ОЧИЩЕННЯ КОШИКА]");

            int result = SHEmptyRecycleBin(IntPtr.Zero, null, RecycleBinFlags.NoConfirmation | RecycleBinFlags.NoProgressUI | RecycleBinFlags.NoSound);
            if (result != 0)
            {
                throw new InvalidOperationException($"Не вдалося очистити кошик. Код помилки: {result}");
            }

            log.AppendLine("[КОШИК ОЧИЩЕНО]");
        }

        private long CleanDirectory(string targetDirectory, StringBuilder log)
        {
            long freedBytes = 0;

            if (!Directory.Exists(targetDirectory))
            {
                return 0;
            }

            log.AppendLine($"[СКАНУЮ ПАПКУ] {targetDirectory}");

            string[] files;
            try
            {
                files = Directory.GetFiles(targetDirectory);
            }
            catch (UnauthorizedAccessException)
            {
                log.AppendLine($"[ЗАБОРОНЕНО ДОСТУП] Немає прав для читання папки: {targetDirectory}");
                return 0;
            }
            catch (Exception ex)
            {
                log.AppendLine($"[ПОМИЛКА] {targetDirectory} - {ex.Message}");
                return 0;
            }

            foreach (string file in files)
            {
                try
                {
                    FileInfo fileInfo = new(file);
                    long size = fileInfo.Length;
                    fileInfo.Delete();
                    freedBytes += size;
                    log.AppendLine($"[ВИДАЛЕНО] {file} ({FormatBytes(size)})");
                }
                catch (Exception ex)
                {
                    log.AppendLine($"[ПРОПУЩЕНО] {file} - {ex.Message}");
                }
            }

            string[] directories;
            try
            {
                directories = Directory.GetDirectories(targetDirectory);
            }
            catch
            {
                return freedBytes;
            }

            foreach (string directory in directories)
            {
                try
                {
                    freedBytes += CleanDirectory(directory, log);
                    Directory.Delete(directory, false);
                    log.AppendLine($"[ПАПКУ ВИДАЛЕНО] {directory}");
                }
                catch (Exception ex)
                {
                    log.AppendLine($"[ПРОПУЩЕНО ПАПКУ] {directory} - {ex.Message}");
                }
            }

            return freedBytes;
        }

        private void SaveLogFile(string logContent, long totalFreed, bool recycleBinCleaned)
        {
            try
            {
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string fullPath = Path.Combine(appPath, "Sonic_Log.json");
                string recycleBinLine = recycleBinCleaned ? "КОШИК: ОЧИЩЕНО" : "КОШИК: НЕ ОЧИЩАВСЯ";
                string finalLog = logContent + $"\n=========================================\nЗАГАЛОМ ЗВІЛЬНЕНО: {FormatBytes(totalFreed)}\n{recycleBinLine}\n\n\n";
                File.AppendAllText(fullPath, finalLog, Encoding.UTF8);
            }
            catch
            {
            }
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

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, RecycleBinFlags dwFlags);

        [Flags]
        private enum RecycleBinFlags : uint
        {
            NoConfirmation = 0x00000001,
            NoProgressUI = 0x00000002,
            NoSound = 0x00000004
        }
    }

    public readonly record struct CleanupResult(bool IsSuccess, string Message);
}
