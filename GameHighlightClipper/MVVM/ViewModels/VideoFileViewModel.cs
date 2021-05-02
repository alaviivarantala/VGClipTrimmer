using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models;
using GameHighlightClipper.MVVM.Models.Interfaces;
using System;
using System.Collections.ObjectModel;
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

        private int _processingProgress;
        public int ProcessingProgress
        {
            get => _processingProgress;
            set => Set(ref _processingProgress, value);
        }

        private int _maxProgress;
        public int MaxProgress
        {
            get => _maxProgress;
            set => Set(ref _maxProgress, value);
        }

        private ObservableCollection<Timeline> _timelines = new ObservableCollection<Timeline>();
        public ObservableCollection<Timeline> Timelines
        {
            get => _timelines;
            set => Set(ref _timelines, value);
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
            MaxProgress = videoFile.VideoLength;

            Timeline first = new Timeline();
            first.Duration = new TimeSpan(1, 0, 0);
            first.Events.Add(new TimelineEvent() { Start = new TimeSpan(0, 15, 0), Duration = new TimeSpan(0, 15, 0) });
            first.Events.Add(new TimelineEvent() { Start = new TimeSpan(0, 40, 0), Duration = new TimeSpan(0, 10, 0) });
            Timelines.Add(first);

            Timeline second = new Timeline();
            second.Duration = new TimeSpan(1, 0, 0);
            second.Events.Add(new TimelineEvent() { Start = new TimeSpan(0, 0, 0), Duration = new TimeSpan(0, 25, 0) });
            second.Events.Add(new TimelineEvent() { Start = new TimeSpan(0, 30, 0), Duration = new TimeSpan(0, 15, 0) });
            second.Events.Add(new TimelineEvent() { Start = new TimeSpan(0, 50, 0), Duration = new TimeSpan(0, 10, 0) });
            Timelines.Add(second);
        }

        private void OpenFileLocation()
        {
            Process.Start("explorer.exe", "/select, \"" + VideoFile.FilePath + "\"");
        }

        public async void ProcessVideo()
        {
            ProgressBarText = "Reading video file info...";
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = tokenSource.Token;
            IProgress<int> videoProcessProgress = new Progress<int>(update => { ProcessingProgress += update; });
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var res = await Task.Run(() => _videoProcessingService.ProcessVideoFileYield(VideoFile, videoProcessProgress, cancelToken));
            stopwatch.Stop();
            ProgressBarText = "Processing video file...";
            var x1 = stopwatch.Elapsed;
            ProcessingProgress = 0;
            stopwatch.Reset();
            stopwatch.Start();
            var results = await Task.Run(() => _videoProcessingService.ProcessVideoFile(VideoFile, videoProcessProgress, cancelToken));
            stopwatch.Stop();
            var x2 = stopwatch.Elapsed;
        }
    }
}