using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models;
using GameHighlightClipper.MVVM.Models.Interfaces;
using System;
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
        public RelayCommand<MouseEventArgs> OpenMediaPlayerCommand => new RelayCommand<MouseEventArgs>((e) => OpenMediaPlayerAction(e), (e) => true);

        #region Actions

        private void OpenFileLocationAction() => OpenFileLocation();
        private void StartProcessingAction() => ProcessVideo();
        private void OpenMediaPlayerAction(MouseEventArgs e) => OpenMediaPlayer(e);
        
        #endregion Actions

        #endregion Commands

        public VideoFileViewModel(INLogLogger nLogLogger, IVideoProcessingService videoProcessingService, VideoFile videoFile)
        {
            _nLogLogger = nLogLogger;
            _videoProcessingService = videoProcessingService;

            VideoFile = videoFile;
            ProcessingProgress = videoFile.Processed;
            ProcessingProgress = videoFile.VideoLength;

            GetVideoFileInfo();
        }

        private void OpenFileLocation()
        {
            Process.Start("explorer.exe", "/select, \"" + VideoFile.FilePath + "\"");
        }

        private void OpenMediaPlayer(MouseEventArgs e)
        {
            var x1 = Utilities.GetPathForExe("VLC.exe");

            if (e.OriginalSource is System.Windows.Shapes.Rectangle rectangle)
            {
                if (rectangle.DataContext is TimelineEvent timelineEvent)
                {
                    if (!string.IsNullOrWhiteSpace(x1))
                    {
                        Process proc = new Process();
                        proc.StartInfo.FileName = x1;
                        proc.StartInfo.Arguments = "\"" + VideoFile.FilePath + "\" --start-time " + (timelineEvent.Start.TotalSeconds - 10) + " --stop-time " + (timelineEvent.Start.TotalSeconds + 10);
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
                MaxProgress = VideoFile.VideoLength * 2;
                ProgressBarText = "Read video file info!";
                _canStartProcessing = true;
                StartProcessingCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                _nLogLogger.LogTrace(ex, "Error in GetVideoFileInfo!");
                ProgressBarText = "!!!ERROR READING VIDEO FILE!!!";
            }
        }

        public async void ProcessVideo()
        {
            try
            {
                _canStartProcessing = false;
                StartProcessingCommand.RaiseCanExecuteChanged();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Timeline.Duration = new TimeSpan(0, 0, MaxProgress / 2);
                ProgressBarText = "Processing video file Step 1: FFMPEG";
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                CancellationToken cancelToken = tokenSource.Token;
                IProgress<int> videoProcessProgress = new Progress<int>(update => 
                { 
                    ProcessingProgress += update;
                    if (ProcessingProgress == MaxProgress / 2)
                    {
                        ProgressBarText = "Processing video file Step 2: Tesseract-OCR";
                    }
                });

                var results = await Task.Run(() => _videoProcessingService.ProcessVideoFile(VideoFile, videoProcessProgress, cancelToken));

                foreach (var result in results)
                {
                    Timeline.Events.Add(new TimelineEvent() { Start = result, Duration = new TimeSpan(0, 0, 5) });
                }
                ProgressBarText = "Video file processed in " + RoundSeconds(stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                _nLogLogger.LogError(ex, "VideoFileViewModel; ProcessVideo");
                ProgressBarText = "!!!Error processing video file!!!";
            }
        }

        public static TimeSpan RoundSeconds(TimeSpan span)
        {
            return TimeSpan.FromSeconds(Math.Round(span.TotalSeconds));
        }
    }
}