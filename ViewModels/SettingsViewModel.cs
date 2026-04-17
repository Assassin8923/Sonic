using System;
using Sonic.Models;

namespace Sonic.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly Action<bool> _applyTheme;
        private double _themeSliderValue;

        public SettingsViewModel(bool isDarkTheme, Action<bool> applyTheme)
        {
            _applyTheme = applyTheme;
            Profile = new UserProfile();
            _themeSliderValue = isDarkTheme ? 0 : 1;
        }

        public UserProfile Profile { get; }

        public double ThemeSliderValue
        {
            get => _themeSliderValue;
            set
            {
                if (SetProperty(ref _themeSliderValue, value))
                {
                    OnPropertyChanged(nameof(ThemeLabel));
                    _applyTheme(value == 0);
                }
            }
        }

        public string ThemeLabel => ThemeSliderValue == 0 ? "Темна" : "Світла";
    }
}
