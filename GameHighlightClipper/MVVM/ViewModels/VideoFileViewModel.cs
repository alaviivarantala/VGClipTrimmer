using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models;
using GameHighlightClipper.MVVM.Models.Interfaces;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GameHighlightClipper.MVVM.ViewModels
{
    public class VideoFileViewModel : ViewModelBase
    {
        private INLogLogger _nLogLogger;
        private IVideoProcessingService _videoProcessingService;

        private VideoFile _videoFile;
        public VideoFile VideoFile
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

        private int _processingProgress = 0;
        public int ProcessingProgress
        {
            get => _processingProgress;
            set => Set(ref _processingProgress, value);
        }

        private int _maxProgress = 1;
        public int MaxProgress
        {
            get => _maxProgress;
            set => Set(ref _maxProgress, value);
        }

        private Timeline _timeline = new Timeline();
        public Timeline Timeline
        {
            get => _timeline;
            set => Set(ref _timeline, value);
        }

        #region Commands

        public RelayCommand OpenFileLocationCommand => new RelayCommand(OpenFileLocationAction);
        public RelayCommand StartProcessingCommand => new RelayCommand(StartProcessingAction);

        #region Actions

        private void OpenFileLocationAction() => OpenFileLocation();
        private void StartProcessingAction() => ProcessVideo();

        #endregion Actions

        #endregion Commands

        public VideoFileViewModel(INLogLogger nLogLogger, IVideoProcessingService videoProcessingService, VideoFile videoFile)
        {
            _nLogLogger = nLogLogger;
            _videoProcessingService = videoProcessingService;

            VideoFile = videoFile;
            ProcessingProgress = videoFile.Processed;
            ProcessingProgress = videoFile.VideoLength;
            ProgressBarText = "Waiting";
        }

        private void OpenFileLocation()
        {
            Process.Start("explorer.exe", "/select, \"" + VideoFile.FilePath + "\"");
        }

        public async void ProcessVideo()
        {
            ProgressBarText = "Reading video file info...";
            VideoFile = _videoProcessingService.GetVideoFileInfo(VideoFile);
            MaxProgress = VideoFile.VideoLength;
            Timeline.Duration = new TimeSpan(0, 0, MaxProgress);
            ProgressBarText = "Processing video file...";
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = tokenSource.Token;
            IProgress<int> videoProcessProgress = new Progress<int>(update => { ProcessingProgress += update; });

            var results = await Task.Run(() => _videoProcessingService.ProcessVideoFile(VideoFile, videoProcessProgress, cancelToken));

            foreach (var result in results)
            {
                Timeline.Events.Add(new TimelineEvent() { Start = result, Duration = new TimeSpan(0, 0, 5) });
            }
            ProgressBarText = "Video file processed!";
        }
    }
}