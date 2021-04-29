using AdonisUI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GameHighlightClipper.MVVM.Models.Interfaces;
using System.Reflection;
using System.Windows;

namespace GameHighlightClipper.MVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private INLogLogger _nLogLogger;
        private IVideoProcessingService _videoProcessingService;

        private bool _isEnglishLanguange = true;
        private bool _isDarkTheme = true;

        private string _windowTitle;
        public string WindowTitle
        {
            get => _windowTitle;
            set => Set(ref _windowTitle, value);
        }

        private string _languageSymbol = "🇪🇳";
        public string LanguageSymbol
        {
            get => _languageSymbol;
            set => Set(ref _languageSymbol, value);
        }

        #region Commands

        public RelayCommand ToggleLanguageCommand => new RelayCommand(ToggleLanguageAction);
        public RelayCommand ToggleThemeCommand => new RelayCommand(ToggleThemeAction);

        #region Actions

        private void ToggleLanguageAction() => ToggleLanguage();
        private void ToggleThemeAction() => ToggleTheme();

        #endregion Actions

        #endregion Commands

        public MainWindowViewModel(INLogLogger nLogLogger, IVideoProcessingService videoProcessingService)
        {
            _nLogLogger = nLogLogger;
            _videoProcessingService = videoProcessingService;
            WindowTitle = "Game Highlight Clipper - " + Assembly.GetExecutingAssembly().GetName().Version;
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