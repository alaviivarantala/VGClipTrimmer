using System;
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
            Stopwatch watch = new Stopwatch();

            watch.Start();

            TestOCRMulti();

            watch.Stop();

            ShutdownApp();
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
