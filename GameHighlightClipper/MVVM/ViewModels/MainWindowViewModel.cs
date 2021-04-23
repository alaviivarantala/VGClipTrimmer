using AdonisUI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Reflection;
using System.Windows;

namespace GameHighlightClipper.MVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isEnglishLanguange = true;
        private bool _isDarkTheme = true;

        private bool _displayDropZone = false;
        public bool DisplayDropZone
        {
            get => _displayDropZone;
            set => Set(ref _displayDropZone, value);
        }
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

        public RelayCommand<DragEventArgs> PreviewDragEnterCommand => new RelayCommand<DragEventArgs>((e) => PreviewDragEnterAction(e), (e) => true);
        public RelayCommand<DragEventArgs> PreviewDragLeaveCommand => new RelayCommand<DragEventArgs>((e) => PreviewDragLeaveAction(e), (e) => true);
        public RelayCommand<DragEventArgs> PreviewDropCommand => new RelayCommand<DragEventArgs>((e) => PreviewDropAction(e), (e) => true);

        public RelayCommand ToggleLanguageCommand => new RelayCommand(ToggleLanguageAction);
        public RelayCommand ToggleThemeCommand => new RelayCommand(ToggleThemeAction);

        #region Actions

        private void PreviewDragEnterAction(DragEventArgs e) => PreviewDragEnter(e);
        private void PreviewDragLeaveAction(DragEventArgs e) => PreviewDragLeave(e);
        private void PreviewDropAction(DragEventArgs e) => PreviewDrop(e);

        private void ToggleLanguageAction() => ToggleLanguage();
        private void ToggleThemeAction() => ToggleTheme();

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            WindowTitle = "Game Highlight Clipper - " + Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void PreviewDragEnter(DragEventArgs e)
        {
            DisplayDropZone = true;
            e.Handled = true;
        }

        private void PreviewDragLeave(DragEventArgs e)
        {
            DisplayDropZone = false;
            e.Handled = true;
        }

        private void PreviewDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                //ParseFiles(paths);

                DisplayDropZone = false;
                //CommandManager.InvalidateRequerySuggested();
                e.Handled = true;
            }
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