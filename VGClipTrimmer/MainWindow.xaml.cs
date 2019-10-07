using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tesseract;
using VGClipTrimmer.helpers;

namespace VGClipTrimmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        readonly string imgPath = "../../img/";
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

            string video = clips + "APEX.mp4";

            string[] lines = FFmpeg.Info(video).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            int ww = int.Parse(lines.Where(x => x.Contains("width=")).FirstOrDefault().Split('=')[1]);
            int hh = int.Parse(lines.Where(x => x.Contains("height=")).FirstOrDefault().Split('=')[1]);

            string[] duration = lines.Where(x => x.Contains("duration=")).FirstOrDefault().Split('=')[1].Split('.')[0].Split(':');
            int length = (int)new TimeSpan(int.Parse(duration[0]), int.Parse(duration[1]), int.Parse(duration[2])).TotalSeconds;

            double widthMultiplier = 0.25;
            double heightMultiplier = 0.10;
            int width = (int)(ww * widthMultiplier);
            int height = (int)(hh * heightMultiplier);
            int startingPointX = (int)((ww / 2) - (width / 2));
            int startingPointY = (int)((hh * 0.75) - (height / 2));

            int maxDegreeOfParallelism = Environment.ProcessorCount;
            Parallel.For(0, length, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                TimeSpan time = TimeSpan.FromSeconds(i);
                Bitmap resultImage = FFmpeg.Snapshot(time.ToString(), video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString());
                string resultOCR = TestOCRImage(resultImage);
            });
            
            watch.Stop();

            ShutdownApp();
        }

        private string TestOCRImage(Bitmap image)
        {
            image = ImageProcessing.ResizeImageSlow(image, 800, 180);
            image = ImageProcessing.CleanImage(image);
            return OCR(image);
        }

        private void TestOCRMulti()
        {
            var testFiles = Directory.EnumerateFiles(imgPath);

            var maxDegreeOfParallelism = Environment.ProcessorCount;
            Parallel.ForEach(testFiles, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (fileName) =>
            {
                var imageFile = File.ReadAllBytes(fileName);
                var texts = TestOCR(fileName);
            });
        }

        private string TestOCR(string path)
        {
            Bitmap image = new Bitmap(path);
            image = ImageProcessing.CropImage(image);
            image = ImageProcessing.ResizeImageSlow(image, 800, 180);
            image = ImageProcessing.CleanImage(image);
            return OCR(image);
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
    }
}
