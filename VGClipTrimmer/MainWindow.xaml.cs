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

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            TestBatch();
        }

        private void TestBatch()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            List<TimeSpan> results = new List<TimeSpan>();

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
            //
            /*int maxDegreeOfParallelism = Environment.ProcessorCount;
            Parallel.For(0, length, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                TimeSpan time = TimeSpan.FromSeconds(i);
                Bitmap resultImage = FFmpeg.Snapshot(time.ToString(), video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString());
                SaveImage(resultImage);
                string resultOCR = TestOCRImage(resultImage);
                if (resultOCR.ToLower().Contains("eliminated") || resultOCR.ToLower().Contains("knocked"))
                {
                    results.Add(time);
                }
            });*/
            //
            List<string> images = new List<string>();
            string data = string.Empty;
            string received = string.Empty;
            string dd = string.Empty;
            DataReceivedEventHandler outputHandler = new DataReceivedEventHandler((sender, e) =>
            {
                received = e.Data;
                if (!string.IsNullOrWhiteSpace(data) && !string.IsNullOrWhiteSpace(received) && received.Contains("PNG"))
                {
                    int index = received.IndexOf("‰PNG");
                    data += received.Substring(0, index);
                    images.Add(data);
                    data = received.Substring(index, received.Length - index);
                }
                else
                {
                    data += received;
                }
            });

            FFmpeg.Snapshots(video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString(), outputHandler);

            for (int i = 0; i < images.Count; i++)
            {
                Bitmap bitmap = new Bitmap(new MemoryStream(Encoding.UTF8.GetBytes(images[i])));
                SaveImage(bitmap);
                string resultOCR = TestOCRImage(bitmap);
                if (resultOCR.ToLower().Contains("eliminated") || resultOCR.ToLower().Contains("knocked"))
                {
                    TimeSpan time = TimeSpan.FromSeconds(i);
                    results.Add(time);
                }
            }

            watch.Stop();
            ShutdownApp();
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
    }
}
