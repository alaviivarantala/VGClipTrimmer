using GalaSoft.MvvmLight;
using System.Drawing;

namespace GameHighlightClipper.MVVM.Models
{
    public class VideoFile : ObservableObject
    {
        public string MD5Sum { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double FrameRate { get; set; }
        public int Processed { get; set; }

        private int _videoLength;
        public int VideoLength
        {
            get => _videoLength;
            set => Set(ref _videoLength, value);
        }

        private Bitmap _thumbnail;
        public Bitmap Thumbnail
        {
            get => _thumbnail;
            set => Set(ref _thumbnail, value);
        }

        public VideoFile()
        {
            Processed = 0;
        }
    }
}