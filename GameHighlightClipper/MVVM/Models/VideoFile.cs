namespace GameHighlightClipper.MVVM.Models
{
    public class VideoFile
    {
        public string MD5Sum { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double FrameRate { get; set; }
        public int VideoLength { get; set; }
        public int Processed { get; set; }

        public VideoFile()
        {
            Processed = 0;
        }
    }
}
