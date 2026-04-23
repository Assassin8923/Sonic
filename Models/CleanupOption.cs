using Sonic.ViewModels;

namespace Sonic.Models
{
    public enum CleanupTargetKind
    {
        DirectoryGroup,
        RecycleBin
    }

    public class CleanupOption : BindableBase
    {
        private bool _isSelected;

        public CleanupOption(string title, string path, CleanupTargetKind targetKind = CleanupTargetKind.DirectoryGroup, bool isAvailable = true)
        {
            Title = title;
            Path = path;
            TargetKind = targetKind;
            IsAvailable = isAvailable;
        }

        public string Title { get; }

        public string Path { get; }

        public CleanupTargetKind TargetKind { get; }

        public bool IsAvailable { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
