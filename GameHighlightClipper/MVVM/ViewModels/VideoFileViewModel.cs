using GalaSoft.MvvmLight;
using GameHighlightClipper.MVVM.Models;
using GameHighlightClipper.MVVM.Models.Interfaces;

namespace GameHighlightClipper.MVVM.ViewModels
{
    public class VideoFileViewModel : ViewModelBase
    {
        private INLogLogger _nLogLogger;

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

        public VideoFileViewModel(INLogLogger nLogLogger)
        {
            _nLogLogger = nLogLogger;
        }
    }
}
