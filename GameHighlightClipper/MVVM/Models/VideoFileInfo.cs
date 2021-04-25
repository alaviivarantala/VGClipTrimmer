namespace GameHighlightClipper.MVVM.Models
{
    public class VideoFileInfo
    {
        public string MD5Sum { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double FrameRate { get; set; }
        public int VideoLength { get; set; }
        public int Processed { get; set; }

        public VideoFileInfo()
        {
            Processed = 0;
        }
    }
}
