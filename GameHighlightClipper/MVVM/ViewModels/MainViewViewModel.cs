using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Diagnostics;
using System.Threading.Tasks;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models.Interfaces;
using GameHighlightClipper.MVVM.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace GameHighlightClipper.MVVM.ViewModels
{
    public class MainViewViewModel : ViewModelBase
    {
        private INLogLogger _nLogLogger;
        private IVideoProcessingService _videoProcessingService;

        private ObservableCollection<VideoFileViewModel> _videoFiles = new ObservableCollection<VideoFileViewModel>();

        public ObservableCollection<VideoFileViewModel> VideoFiles
        {
            get => _videoFiles;
            set => Set(ref _videoFiles, value);
        }

        #region Commands

        public RelayCommand BrowseForFilesCommand => new RelayCommand(BrowseForFilesAction);
        public RelayCommand StartProcessingAllCommand => new RelayCommand(StartProcessingAllAction);

        #region Actions

        private void BrowseForFilesAction() => BrowseForFiles();
        private void StartProcessingAllAction() => ProcessAllVideos();

        #endregion Actions

        #endregion Commands

        public MainViewViewModel(INLogLogger nLogLogger, IVideoProcessingService videoProcessingService)
        {
            _nLogLogger = nLogLogger;
            _videoProcessingService = videoProcessingService;
        }

        private void BrowseForFiles()
        {
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
                            FileSize = FileTools.GetFileSize(videoFilePath)
                        };
                        VideoFileViewModel viewModel = new VideoFileViewModel(_nLogLogger);
                        viewModel.VideoFile = videoFile;
                        VideoFiles.Add(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _nLogLogger.LogError(ex, "MainViewViewModel; BrowseForFiles");
            }
        }

        private async void ProcessAllVideos()
        {
            await Task.Delay(1000);
            //VideoFileInfo videoFileInfo = await _videoProcessingService.GetVideoFileInfo(VideoFile);
            /*
            List<Task<VideoFileInfo>> taskList = new List<Task<VideoFileInfo>>();
            taskList.Add(Task.Run(() => _videoProcessingService.GetVideoFileInfo(VideoFile)));
            await Task.WhenAll(taskList);
            VideoFileInfo videoFileInfo = taskList[0].Result;
            await Task.Delay(1000);
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = tokenSource.Token;
            IProgress<int> videoProcessProgress = new Progress<int>(update => { ProcessingProgress += update; });
            await _videoProcessingService.ProcessVideoFile(VideoFile, videoProcessProgress, cancelToken);
            */
        }
    }
}