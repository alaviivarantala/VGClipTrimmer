using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models.Interfaces;
using GameHighlightClipper.MVVM.Models.Services;

namespace GameHighlightClipper.MVVM.ViewModels
{
    public class MainViewViewModel : ViewModelBase
    {
        private IVideoProcessingService _videoProcessingService;

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
            VideoFile = General.SelectVideoFiles()[0];
        }

        private void OpenFileLocation()
        {
            Process.Start("explorer.exe", "/select, \"" + VideoFile + "\"");
        }

        private async void ProcessVideo()
        {
            await Task.Delay(1000);
            //VideoFileInfo videoFileInfo = await _videoProcessingService.GetVideoFileInfo(VideoFile);

            List<Task<VideoFileInfo>> taskList = new List<Task<VideoFileInfo>>();
            taskList.Add(Task.Run(() => _videoProcessingService.GetVideoFileInfo(VideoFile)));
            await Task.WhenAll(taskList);
            VideoFileInfo videoFileInfo = taskList[0].Result;
            await Task.Delay(1000);
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = tokenSource.Token;
            IProgress<int> videoProcessProgress = new Progress<int>(update => { ProcessingProgress += update; });
            await _videoProcessingService.ProcessVideoFile(VideoFile, videoProcessProgress, cancelToken);
        }


    }
}
