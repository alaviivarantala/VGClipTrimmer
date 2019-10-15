using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Tesseract;
using VGClipTrimmer.helpers;

namespace VGClipTrimmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        readonly string clips = "C:/clips/";

        private void ShutdownApp()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            await Task.Run(() => TestBatch());
            //ShutdownApp();
        }

        private void TestBatch()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            string video = clips + "APEX.mp4";

            string[] lines = FFmpeg.Info(video).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            int ww = int.Parse(lines.Where(x => x.Contains("width=")).FirstOrDefault().Split('=')[1]);
            int hh = int.Parse(lines.Where(x => x.Contains("height=")).FirstOrDefault().Split('=')[1]);

            string[] duration = lines.Where(x => x.Contains("duration=")).FirstOrDefault().Split('=')[1].Split('.')[0].Split(':');
            int length = (int)new TimeSpan(int.Parse(duration[0]), int.Parse(duration[1]), int.Parse(duration[2])).TotalSeconds;

            double widthMultiplier = 0.25;
            double heightMultiplier = 0.05;
            int width = (int)(ww * widthMultiplier / 1.75);
            int height = (int)(hh * heightMultiplier);
            int startingPointX = (int)((ww / 2) - (width / 1.4));
            int startingPointY = (int)((hh * 0.75) - (height / 0.9));

            List<Tuple<int, bool>> tuples = new List<Tuple<int, bool>>();

            EventHandler handler = new EventHandler((sender, e) =>
            {
                ImagesEventArgs args = e as ImagesEventArgs;
                tuples.Add(OCRImage(args.Seconds, args.Image));
            });

            Stopwatch watch1 = new Stopwatch();
            watch1.Start();
            List<byte[]> images = FFmpeg.SnapshotsToList(video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString(), 307200);
            watch1.Stop();
            Stopwatch watch2 = new Stopwatch();
            watch2.Start();
            int maxDegreeOfParallelism = Environment.ProcessorCount;
            Parallel.For(0, images.Count, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                tuples.Add(OCRImage(i, images[i]));
            });
            watch2.Stop();

            List<TimeSpan> results = tuples.Where(result => result.Item2).Select(time => TimeSpan.FromSeconds(time.Item1)).ToList();

            watch.Stop();
            int xx = 0;
        }

        private Tuple<int, bool> OCRImage(int seconds, byte[] imageBytes)
        {
            bool result = false;
            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                using (Bitmap image = new Bitmap(memoryStream))
                {
                    string resultOCR = TestOCRImage(image);
                    if (resultOCR.ToLower().Contains("eliminated") || resultOCR.ToLower().Contains("knocked"))
                    {
                        TimeSpan time = TimeSpan.FromSeconds(seconds);
                        result = true;
                    }
                }
            }
            return new Tuple<int, bool>(seconds, result);
        }

        private string TestOCRImage(Bitmap image)
        {
            return OCR(ResizeCleanImage(image));
        }

        private Bitmap ResizeCleanImage(Bitmap image)
        {
            image = ImageProcessing.ResizeImageSlow(image, 800, 158);
            image = ImageProcessing.CleanImage(image);
            return image;
        }

        private string OCR(Bitmap img)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                // Whitelist of chars to recognize
                engine.SetVariable("tessedit_char_whitelist", "ACDEIKLMNOTW");
                // Dont bother with word plausibility
                engine.SetVariable("tessedit_unrej_any_wd", true);

                Page page = engine.Process(img, PageSegMode.Auto);
                res = page.GetText();
            }
            return res;
        }

        private void SaveImage(Bitmap image)
        {
            Bitmap original = new Bitmap(image);
            original.Save(clips + "output1.png");

            Bitmap resized = new Bitmap(ImageProcessing.ResizeImageSlow(image, 800, 158));
            resized.Save(clips + "output2.png");

            Bitmap cleaned = new Bitmap(ResizeCleanImage(image));
            cleaned.Save(clips + "output3.png");
        }

        private void SaveImage(Image image, string name)
        {
            using (Bitmap temp = new Bitmap(image))
            {
                temp.Save(clips + "tempb/" + name + ".png");
            }
        }
    }
}
