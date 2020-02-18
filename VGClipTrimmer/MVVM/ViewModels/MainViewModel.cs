using AdonisUI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VGClipTrimmer.Helpers;
using VGClipTrimmer.MVVM.Models.Interfaces;
using VGClipTrimmer.MVVM.Models.Services;

namespace VGClipTrimmer.MVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isEnglishLanguange = true;
        private bool _isDarkTheme = true;

        private IVideoProcessingService _videoProcessingService;

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
        private string _videoFile;
        public string VideoFile
        {
            get => _videoFile;
            set => Set(ref _videoFile, value);
        }
        private string _progressBarText;
        public string ProgressBarText
        {
            get => _progressBarText;
            set => Set(ref _progressBarText, value);
        }
        private int _processingProgress;
        public int ProcessingProgress
        {
            get => _processingProgress;
            set => Set(ref _processingProgress, value);
        }

        public RelayCommand ToggleLanguageCommand => new RelayCommand(ToggleLanguageAction);
        public RelayCommand ToggleThemeCommand => new RelayCommand(ToggleThemeAction);
        public RelayCommand OpenFileLocationCommand => new RelayCommand(OpenFileLocationAction);
        public RelayCommand BrowseForFilesCommand => new RelayCommand(BrowseForFilesAction);
        public RelayCommand StartProcessingCommand => new RelayCommand(StartProcessingAction);

        private void ToggleLanguageAction() => ToggleLanguage();
        private void ToggleThemeAction() => ToggleTheme();
        private void OpenFileLocationAction() => Process.Start("explorer.exe", "/select, \"" + VideoFile + "\"");
        private void BrowseForFilesAction() => VideoFile = General.SelectVideoFile();
        private void StartProcessingAction() => ProcessVideo();

        public MainViewModel()
        {
            _videoProcessingService = new VideoProcessingService();
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

        private async void ProcessVideo()
        {
            VideoFileInfo videoFileInfo = _videoProcessingService.GetVideoFileInfo(VideoFile);

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = tokenSource.Token;
            IProgress<int> videoProcessProgress = new Progress<int>(update => { ProcessingProgress += update; });
            await Task.Run(() => _videoProcessingService.ProcessVideoFile(VideoFile, videoProcessProgress, cancelToken));
        }
    }
}
