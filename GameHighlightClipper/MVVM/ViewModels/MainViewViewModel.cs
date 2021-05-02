using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Threading.Tasks;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models.Interfaces;
using GameHighlightClipper.MVVM.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace GameHighlightClipper.MVVM.ViewModels
{
    public class MainViewViewModel : ViewModelBase
    {
        private INLogLogger _nLogLogger;
        private IVideoProcessingService _videoProcessingService;

        private bool _displayDropZone = false;
        public bool DisplayDropZone
        {
            get => _displayDropZone;
            set => Set(ref _displayDropZone, value);
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

        private ObservableCollection<VideoFileViewModel> _videoFiles = new ObservableCollection<VideoFileViewModel>();
        public ObservableCollection<VideoFileViewModel> VideoFiles
        {
            get => _videoFiles;
            set => Set(ref _videoFiles, value);
        }

        #region Commands

        public RelayCommand<DragEventArgs> PreviewDragEnterCommand => new RelayCommand<DragEventArgs>((e) => PreviewDragEnterAction(e), (e) => true);
        public RelayCommand<DragEventArgs> PreviewDragLeaveCommand => new RelayCommand<DragEventArgs>((e) => PreviewDragLeaveAction(e), (e) => true);
        public RelayCommand<DragEventArgs> PreviewDropCommand => new RelayCommand<DragEventArgs>((e) => PreviewDropAction(e), (e) => true);

        public RelayCommand BrowseForFilesCommand => new RelayCommand(BrowseForFilesAction);
        public RelayCommand StartProcessingAllCommand => new RelayCommand(StartProcessingAllAction);

        #region Actions

        private void PreviewDragEnterAction(DragEventArgs e) => PreviewDragEnter(e);
        private void PreviewDragLeaveAction(DragEventArgs e) => PreviewDragLeave(e);
        private void PreviewDropAction(DragEventArgs e) => PreviewDrop(e);

        private void BrowseForFilesAction() => BrowseForFiles();
        private void StartProcessingAllAction() => ProcessAllVideos();

        #endregion Actions

        #endregion Commands

        public MainViewViewModel(INLogLogger nLogLogger, IVideoProcessingService videoProcessingService)
        {
            _nLogLogger = nLogLogger;
            _videoProcessingService = videoProcessingService;
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

        private void BrowseForFiles()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                string[] videoFilePaths = FileTools.SelectVideoFiles();

                foreach (string videoFilePath in videoFilePaths)
                {
                    if (!string.IsNullOrWhiteSpace(videoFilePath))
                    {
                        VideoFile videoFile = new VideoFile
                        {
                            FileName = new FileInfo(videoFilePath).Name,
                            FilePath = videoFilePath,
                            FileSize = FileTools.GetFileSize(videoFilePath),
                            Processed = 0
                        };
                        VideoFileViewModel viewModel = new VideoFileViewModel(_nLogLogger, _videoProcessingService, videoFile);
                        VideoFiles.Add(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _nLogLogger.LogError(ex, "MainViewViewModel; BrowseForFiles");
            }
            var x = stopwatch.Elapsed;
        }

        private void ProcessAllVideos()
        {
            foreach (VideoFileViewModel videoFile in VideoFiles)
            {
                videoFile.ProcessVideo();
            }
        }
    }
}