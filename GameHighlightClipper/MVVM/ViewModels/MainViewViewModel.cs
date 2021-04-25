using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models.Interfaces;
using GameHighlightClipper.MVVM.Models.Services;
using GameHighlightClipper.MVVM.Models;
using System;

namespace GameHighlightClipper.MVVM.ViewModels
{
    public class MainViewViewModel : ViewModelBase
    {
        private IVideoProcessingService _videoProcessingService;

        private List<VideoFile> _videoFiles;
        public List<VideoFile> VideoFiles
        {
            get => _videoFiles;
            set => Set(ref _videoFiles, value);
        }
        /*
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
        */
        #region Commands

        public RelayCommand BrowseForFilesCommand => new RelayCommand(BrowseForFilesAction);

        public RelayCommand OpenFileLocationCommand => new RelayCommand(OpenFileLocationAction);
        public RelayCommand StartProcessingCommand => new RelayCommand(StartProcessingAction);

        #region Actions

        private void BrowseForFilesAction() => BrowseForFiles();

        private void OpenFileLocationAction() => OpenFileLocation();
        private void StartProcessingAction() => ProcessVideo();

        #endregion

        #endregion

        public MainViewViewModel()
        {
            _videoProcessingService = new VideoProcessingService();
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
                            FilePath = videoFilePath,
                            FileSize = FileTools.GetFileSize(videoFilePath)
                        };
                        VideoFiles.Add(videoFile);
                    }
                }
            }
            catch (Exception ex)
            {
                NLogLogger.LogError(ex, "MainViewViewModel; BrowseForFiles");
            }
        }

        private void OpenFileLocation()
        {
            int index = 0;
            Process.Start("explorer.exe", "/select, \"" + VideoFiles[index].FilePath + "\"");
        }

        private async void ProcessVideo()
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
