using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sonic.Views
{
    public partial class CleanupPage : Page
    {
        public CleanupPage()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private async void CButton_Click(object sender, RoutedEventArgs e)
        {
            Button? clickedBtn = sender as Button;
            if (clickedBtn != null) clickedBtn.IsEnabled = false;

            try
            {
                string userTemp = Path.GetTempPath();
                string systemTemp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp");

                StringBuilder logBuilder = new StringBuilder();
                logBuilder.AppendLine("=========================================");
                logBuilder.AppendLine($"ЗВІТ ПРО ОЧИЩЕННЯ SONIC");
                logBuilder.AppendLine($"Дата: {DateTime.Now}");
                logBuilder.AppendLine("=========================================\n");

                long totalFreedBytes = 0;

                totalFreedBytes += await Task.Run(() => CleanDirectory(userTemp, logBuilder));
                totalFreedBytes += await Task.Run(() => CleanDirectory(systemTemp, logBuilder));

                SaveLogFile(logBuilder.ToString(), totalFreedBytes);

                CustomMessage.ShowMessage("Очищення завершено!", "Sonic");
            }
            catch (Exception ex)
            {
                CustomMessage.ShowMessage($"Виникла критична помилка: {ex.Message}", "Помилка");
            }
            finally
            {
                if (clickedBtn != null) clickedBtn.IsEnabled = true;
            }
        }


        private long CleanDirectory(string targetDirectory, StringBuilder log)
        {
            long freedBytes = 0;

            if (!Directory.Exists(targetDirectory)) return 0;

            log.AppendLine($"[СКАНОМ ПАПКУ] {targetDirectory}");

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
                    FileInfo fi = new FileInfo(file);
                    long size = fi.Length;

                    fi.Delete();

                    freedBytes += size;
                    log.AppendLine($"[ВИДАЛЕНО] {file} ({FormatBytes(size)})");
                }
                catch (Exception ex)
                {
                    log.AppendLine($"[ПРОПУЩЕНО] {file} - {ex.Message}");
                }
            }


            string[] dirs;
            try
            {
                dirs = Directory.GetDirectories(targetDirectory);
            }
            catch
            {
                return freedBytes;
            }

            foreach (string dir in dirs)
            {
                try
                {
                    freedBytes += CleanDirectory(dir, log);
                    Directory.Delete(dir, false);
                    log.AppendLine($"[ПАПКУ ВИДАЛЕНО] {dir}");
                }
                catch (Exception ex)
                {
                    log.AppendLine($"[ПРОПУЩЕНО ПАПКУ] {dir} - {ex.Message}");
                }
            }

            return freedBytes;
        }

        private void SaveLogFile(string logContent, long totalFreed)
        {
            try
            {
                string appPath = AppDomain.CurrentDomain.BaseDirectory;

                string fileName = "Sonic_Log.json";
                string fullPath = Path.Combine(appPath, fileName);

                string finalLog = logContent + $"\n=========================================\nЗАГАЛОМ ЗВІЛЬНЕНО: {FormatBytes(totalFreed)}\n\n\n";

                File.AppendAllText(fullPath, finalLog, Encoding.UTF8);
            }
            catch
            {
            }
        }

        private string FormatBytes(long bytes)
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