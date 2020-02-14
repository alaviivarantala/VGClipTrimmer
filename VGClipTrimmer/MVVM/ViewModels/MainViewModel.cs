using AdonisUI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Diagnostics;
using System.Windows;

namespace VGClipTrimmer.MVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isEnglishLanguange = true;
        private bool _isDarkTheme = true;

        private string _languageSymbol = "🇪🇳";
        public string LanguageSymbol
        {
            get => _languageSymbol;
            set => Set(ref _languageSymbol, value);
        }

        private string _videoFile;
        public string VideoFile
        {
            get => _videoFile;
            set => Set(ref _videoFile, value);
        }

        public RelayCommand ToggleLanguageCommand => new RelayCommand(ToggleLanguageAction);
        public RelayCommand ToggleThemeCommand => new RelayCommand(ToggleThemeAction);
        public RelayCommand OpenFileLocationCommand => new RelayCommand(OpenFileLocationAction);
        public RelayCommand BrowseForFilesCommand => new RelayCommand(BrowseForFilesAction);
        public RelayCommand StartProcessingCommand => new RelayCommand(StartProcessingAction);

        private void ToggleLanguageAction() => ToggleLanguage();
        private void ToggleThemeAction() => ToggleTheme();
        private void OpenFileLocationAction() => Process.Start("explorer.exe", "/select, \"" + VideoFile + "\"");
        private void BrowseForFilesAction() => VideoFile = Helpers.General.SelectVideoFile();
        private void StartProcessingAction()
        {

        }

        public MainViewModel()
        {

        }

        private void ToggleLanguage()
        {
            _isEnglishLanguange = !_isEnglishLanguange;
            LanguageSymbol = _isEnglishLanguange ? "🇪🇳" : "🇫🇮";
        }

        private void ToggleTheme()
        {
            ResourceLocator.SetColorScheme(Application.Current.Resources, _isDarkTheme ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);
            _isDarkTheme = !_isDarkTheme;
        }
    }
}
