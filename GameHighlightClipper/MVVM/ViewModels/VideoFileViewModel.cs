using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models;
using GameHighlightClipper.MVVM.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameHighlightClipper.MVVM.ViewModels
{
    public class VideoFileViewModel : ViewModelBase
    {
        private INLogLogger _nLogLogger;
        private IVideoProcessingService _videoProcessingService;

        private MainViewViewModel _mainViewViewModel;

        private bool _canStartProcessing = false;

        private VideoFile _videoFile = new VideoFile();
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
        public RelayCommand StartProcessingCommand => new RelayCommand(StartProcessingAction, () => _canStartProcessing);
        public RelayCommand RemoveVideoFileCommand => new RelayCommand(RemoveVideoFileAction);
        public RelayCommand<MouseEventArgs> OpenMediaPlayerCommand => new RelayCommand<MouseEventArgs>((e) => OpenMediaPlayerAction(e), (e) => true);

        #region Actions

        private void OpenFileLocationAction() => OpenFileLocation();
        private void StartProcessingAction() => ProcessVideo();
        private void RemoveVideoFileAction() => RemoveVideoFile();
        private void OpenMediaPlayerAction(MouseEventArgs e) => OpenMediaPlayer(e);
        
        #endregion Actions

        #endregion Commands

        public VideoFileViewModel(INLogLogger nLogLogger, IVideoProcessingService videoProcessingService, VideoFile videoFile, MainViewViewModel mainViewViewModel)
        {
            _nLogLogger = nLogLogger;
            _videoProcessingService = videoProcessingService;

            _mainViewViewModel = mainViewViewModel;

            VideoFile = videoFile;
            ProcessingProgress = videoFile.Processed;
            ProcessingProgress = videoFile.VideoLength;

            GetVideoFileInfo();
        }

        private void OpenFileLocation()
        {
            Process.Start("explorer.exe", "/select, \"" + VideoFile.FilePath + "\"");
        }

        private void RemoveVideoFile()
        {
            _mainViewViewModel.VideoFiles.Remove(this);
        }

        private void OpenMediaPlayer(MouseEventArgs e)
        {
            if (e.OriginalSource is System.Windows.Shapes.Rectangle rectangle)
            {
                if (rectangle.DataContext is TimelineEvent timelineEvent)
                {
                    var x1 = Utilities.GetPathForExe("VLC.exe");
                    if (!string.IsNullOrWhiteSpace(x1))
                    {
                        Process proc = new Process();
                        proc.StartInfo.FileName = x1;
                        proc.StartInfo.Arguments = "\"" + VideoFile.FilePath + "\" --start-time " + (timelineEvent.Start.TotalSeconds) + " --stop-time " + (timelineEvent.Start.TotalSeconds+timelineEvent.Duration.TotalSeconds);
                        proc.Start();
                    }
                }
            }
        }

        private async void GetVideoFileInfo()
        {
            try
            {
                ProgressBarText = "Reading video file info...";
                VideoFile = await Task.Run(() => _videoProcessingService.GetVideoFileInfo(VideoFile));
                MaxProgress = VideoFile.VideoLength;
                ProgressBarText = "Ready to process video file.";
                ChangeCanProcessVideo(true);
            }
            catch (Exception ex)
            {
                _nLogLogger.LogTrace(ex, "VideoFileViewModel:GetVideoFileInfo!");
                ProgressBarText = "!!!ERROR READING VIDEO FILE!!!";
            }
        }

        public async void ProcessVideo()
        {
            try
            {
                ChangeCanProcessVideo(false);
                ProgressBarText = "Processing video file, step 1: FFMPEG";
                ProcessingProgress = 0;
                Timeline = new Timeline();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Timeline.Duration = new TimeSpan(0, 0, MaxProgress);
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                CancellationToken cancelToken = tokenSource.Token;
                bool firstStep = true;
                IProgress<int> videoProcessProgress = new Progress<int>(update => 
                { 
                    ProcessingProgress += update;
                    if (ProcessingProgress == MaxProgress && firstStep)
                    {
                        ProgressBarText = "Processing video file, step 2: Tesseract-OCR";
                        firstStep = false;
                        ProcessingProgress = 0;
                    }
                });

                List<TimeSpan> results = await Task.Run(() => _videoProcessingService.ProcessVideoFile(VideoFile, videoProcessProgress, cancelToken));

                Timeline.Events = new ObservableCollection<TimelineEvent>(MergeTimesSpans(results));

                ProgressBarText = "Video file processed in " + TimeSpan.FromSeconds(Math.Round(stopwatch.Elapsed.TotalSeconds));
                ChangeCanProcessVideo(true);
            }
            catch (Exception ex)
            {
                _nLogLogger.LogError(ex, "VideoFileViewModel:ProcessVideo");
                ProgressBarText = "!!!ERROR PROCESSING VIDEO FILE!!!";
            }
        }

        private List<TimelineEvent> MergeTimesSpans(List<TimeSpan> timeSpans)
        {
            List<TimelineEvent> timelineEvents = new List<TimelineEvent>();

            Tuple<TimeSpan, TimeSpan> previous = null;

            foreach (TimeSpan span in timeSpans)
            {
                Tuple<TimeSpan, TimeSpan> current = new Tuple<TimeSpan, TimeSpan>(span.Subtract(new TimeSpan(0, 0, 5)), span.Add(new TimeSpan(0, 0, 5)));
                if (previous == null)
                {
                    previous = current;
                }
                // no overlap
                else if (current.Item1 > previous.Item2)
                {
                    timelineEvents.Add(new TimelineEvent() { Start = previous.Item1, Duration = previous.Item2-previous.Item1 });
                    previous = current;
                }
                // overlap
                else
                {
                    previous = new Tuple<TimeSpan, TimeSpan>(previous.Item1, new TimeSpan(0, 0, (int)Math.Max(previous.Item2.TotalSeconds, current.Item2.TotalSeconds)));
                }
            }
            timelineEvents.Add(new TimelineEvent() { Start = previous.Item1, Duration = previous.Item2 - previous.Item1 });

            return timelineEvents;
        }

        private void ChangeCanProcessVideo(bool canProcess)
        {
            _canStartProcessing = canProcess;
            StartProcessingCommand.RaiseCanExecuteChanged();
        }
    }
}