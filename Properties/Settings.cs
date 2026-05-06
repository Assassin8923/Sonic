using System;
using System.IO;
using System.Text.Json;

namespace Sonic.Properties
{
    public static class Settings
    {
        private static readonly string SettingsPath = Path.Combine(AppContext.BaseDirectory, "settings.json");
        private static AppSettings? _loaded;

        public static bool IsDarkTheme
        {
            get => Load().IsDarkTheme;
            set
            {
                var s = Load();
                s.IsDarkTheme = value;
                Save(s);
            }
        }

        public static double WindowWidth
        {
            get => Load().WindowWidth;
            set
            {
                var s = Load();
                s.WindowWidth = value;
                Save(s);
            }
        }

        public static double WindowHeight
        {
            get => Load().WindowHeight;
            set
            {
                var s = Load();
                s.WindowHeight = value;
                Save(s);
            }
        }

        public static void Save()
        {
            if (_loaded != null)
            {
                Save(_loaded);
            }
        }

        private static AppSettings Load()
        {
            if (_loaded != null)
                return _loaded;

            if (File.Exists(SettingsPath))
            {
                try
                {
                    var json = File.ReadAllText(SettingsPath);
                    _loaded = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                catch
                {
                    _loaded = new AppSettings();
                }
            }
            else
            {
                _loaded = new AppSettings();
            }

            return _loaded;
        }

        private static void Save(AppSettings settings)
        {
            _loaded = settings;
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
    }

    public class AppSettings
    {
        public bool IsDarkTheme { get; set; } = true;
        public double WindowWidth { get; set; } = 800;
        public double WindowHeight { get; set; } = 450;
    }
}
