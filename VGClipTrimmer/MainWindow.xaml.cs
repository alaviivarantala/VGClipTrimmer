using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            List<TimeSpan> durations = new List<TimeSpan>();
            List<TimeSpan> durationsOCR = new List<TimeSpan>();
            List<TimeSpan> durationsFFMPEG = new List<TimeSpan>();
            List<string> results = new List<string>();

            Stopwatch watch = new Stopwatch();

            watch.Start();

            //int ww = 2560;
            //int hh = 1440;
            int ww = 1280;
            int hh = 720;

            double widthMultiplier = 0.25;
            double heightMultiplier = 0.10;
            int width = (int)(ww * widthMultiplier);
            int height = (int)(hh * heightMultiplier);
            int startingPointX = (int)((ww / 2) - (width / 2));
            int startingPointY = (int)((hh * 0.75) - (height / 2));

            //string time = "00:05:23";
            //string time = "00:00:06";
            string video = clips + "APEX.mp4";
            /*
            for (int i = 0; i < 10; i++)
            {
                Stopwatch watch2 = new Stopwatch();
                watch2.Start();
                string time = "00:00:0" + (i).ToString();
                Bitmap resultImage = FFmpegPipe.Video(time, video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString());
                string resultOCR = TestOCRImage(resultImage);
                results.Add(resultOCR);
                durations.Add(watch2.Elapsed);
            }
            */

            var maxDegreeOfParallelism = Environment.ProcessorCount;
            Parallel.For(0, 10, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                Stopwatch total = new Stopwatch();
                total.Start();
                Stopwatch partWatch = new Stopwatch();
                partWatch.Start();
                string time = "00:00:0" + (i).ToString();
                Bitmap resultImage = FFmpegPipe.Video(time, video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString());
                durationsFFMPEG.Add(partWatch.Elapsed);
                partWatch = new Stopwatch();
                partWatch.Start();
                string resultOCR = TestOCRImage(resultImage);
                durationsOCR.Add(partWatch.Elapsed);
                results.Add(resultOCR);
                durations.Add(total.Elapsed);
            });

            Parallel.For(10, 50, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                Stopwatch total = new Stopwatch();
                total.Start();
                Stopwatch partWatch = new Stopwatch();
                partWatch.Start();
                string time = "00:00:" + (i).ToString();
                Bitmap resultImage = FFmpegPipe.Video(time, video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString());
                durationsFFMPEG.Add(partWatch.Elapsed);
                partWatch = new Stopwatch();
                partWatch.Start();
                string resultOCR = TestOCRImage(resultImage);
                durationsOCR.Add(partWatch.Elapsed);
                results.Add(resultOCR);
                durations.Add(total.Elapsed);
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
