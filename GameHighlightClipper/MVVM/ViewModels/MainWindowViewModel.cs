using AdonisUI;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private bool _displayDropZone = false;
        public bool DisplayDropZone
        {
            get => _displayDropZone;
            set => Set(ref _displayDropZone, value);
        }

        private string _languageSymbol = "🇪🇳";
        public string LanguageSymbol
        {
            get => _languageSymbol;
            set => Set(ref _languageSymbol, value);
        }

        private string _dragDropInfo = string.Empty;
        public string DragDropInfo
        {
            get => _dragDropInfo;
            set => Set(ref _dragDropInfo, value);
        }

        private DragDropType _dragDropType;
        public DragDropType DragDropType
        {
            get => _dragDropType;
            set => Set(ref _dragDropType, value);
        }

        private MainViewViewModel _mainView;
        public MainViewViewModel MainView
        {
            get => _mainView;
            set => Set(ref _mainView, value);
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

        #endregion Actions

        #endregion Commands

        public MainWindowViewModel(INLogLogger nLogLogger, IVideoProcessingService videoProcessingService)
        {
            _nLogLogger = nLogLogger;
            _videoProcessingService = videoProcessingService;
            WindowTitle = "Game Highlight Clipper - " + Assembly.GetExecutingAssembly().GetName().Version;
            MainView = new MainViewViewModel(_nLogLogger, _videoProcessingService);
        }

        private void PreviewDragEnter(DragEventArgs e)
        {
            DisplayDropZone = true;
            e.Handled = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                List<Tuple<DragDropType, long>> typesSizesList = new List<Tuple<DragDropType, long>>();

                foreach (string path in paths)
                {
                    // is directory
                    if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                    {
                        var files = Directory.EnumerateFiles(path, ".", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            if (FileTools.IsVideoFile(file))
                            {
                                typesSizesList.Add(new Tuple<DragDropType, long>(DragDropType.Folder, FileTools.GetFileSize(file)));
                            }
                        }
                    }
                    // is file
                    else
                    {
                        if (FileTools.IsVideoFile(path))
                        {
                            typesSizesList.Add(new Tuple<DragDropType, long>(DragDropType.File, FileTools.GetFileSize(path)));
                        }
                    }
                }

                if (typesSizesList.Count > 0)
                {
                    // count total size
                    long totalByteCount = typesSizesList.Sum(x => x.Item2);

                    // only one file or folder
                    if (typesSizesList.Count == 1)
                    {
                        DragDropType = typesSizesList[0].Item1;
                    }
                    // more than one
                    else
                    {
                        if (typesSizesList.GroupBy(x => x.Item1).Select(x => x.First()).ToList().Count == 1)
                        {
                            DragDropType = typesSizesList[0].Item1 + 1;
                        }
                        else
                        {
                            DragDropType = DragDropType.Multiple;
                        }
                    }

                    DragDropInfo = typesSizesList.Count + " valid video file(s), total size: " + FileTools.FormatFileSize(totalByteCount);
                }
                else
                {
                    DragDropInfo = "No valid files.";
                    DragDropType = DragDropType.Invalid;
                }
            }
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
                e.Handled = true;
                //CommandManager.InvalidateRequerySuggested();
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