using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tesseract;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;
using Xabe.FFmpeg.Model;
using Xabe.FFmpeg.Streams;

namespace VGClipTrimmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        readonly string imgPath = "../../img/";
        readonly string clips = "C:/clips/";

        public MainWindow()
        {
            InitializeComponent();

            FFmpeg.ExecutablesPath = "./ffmpeg";
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            TestVideo();
        }

        private async void TestVideo()
        {
            string file = clips + "APEX.mp4";

            List<int> hits = await ProcessVideo(file);

            ShutdownApp();
        }
        
        private async Task<List<int>> ProcessVideo(string file)
        {
            List<int> timestamps = new List<int>();

            IMediaInfo mediaInfo = await MediaInfo.Get(file);
            IVideoStream videoStream = mediaInfo.VideoStreams.FirstOrDefault();
            int count = (int)videoStream.Duration.TotalSeconds;

            List<Task> taskList = new List<Task>();

            for (int i = 0; i < count; i++)
            {
                taskList.Add(ProcessFrame(file, i));
            }

            await Task.WhenAll(taskList);

            return timestamps;
        }

        private async Task<bool> ProcessFrame(string file, int second)
        {
            string output = clips + "temp/" + Guid.NewGuid() + FileExtensions.Png;
            await Conversion.Snapshot(file, output, TimeSpan.FromSeconds(second)).Start();
            Bitmap bitmap = new Bitmap(output);
            string ocrResult = OCRImage(bitmap);
            bitmap.Dispose();
            File.Delete(output);
            if (ocrResult.ToLower().Contains("eliminated") || ocrResult.ToLower().Contains("knocked"))
            {
                return true;
            }
            return false;
        }

        private void ShutdownApp()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void TestBatch()
        {
            string result1 = TestOCR(imgPath + "apex1.png");
            string result2 = TestOCR(imgPath + "apex2.png");
            string result3 = TestOCR(imgPath + "apex3.png");
            string result4 = TestOCR(imgPath + "apex4.png");
            string result5 = TestOCR(imgPath + "apex5.png");
            string result6 = TestOCR(imgPath + "apex6.png");
            string result7 = TestOCR(imgPath + "apex7.png");
        }

        private string TestOCR(string path)
        {
            Bitmap image = new Bitmap(path);
            image = CropImage(image);
            image = ResizeImageSlow(image, 800, 180);
            image = CleanImage(image);

            image.Save(new FileInfo(path).DirectoryName + "\\" + "P" + new FileInfo(path).Name, System.Drawing.Imaging.ImageFormat.Png);
            return OCR3(image);
        }

        private Bitmap CropImage(Bitmap image)
        {
            double widthMultiplier = 0.25;
            double heightMultiplier = 0.10;
            int width = (int)(image.Width * widthMultiplier);
            int height = (int)(image.Height * heightMultiplier);

            int startingPointX = (image.Width / 2) - (width / 2);
            int startingPointY = (int)(image.Height * 0.75) - (height / 2);

            Rectangle section = new Rectangle(new Point(startingPointX, startingPointY), new Size(width, height));
            Bitmap bitmap = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        private Bitmap CleanImage(Bitmap image)
        {
            image = image.Clone(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            GaussianSharpen gs = new GaussianSharpen();
            ContrastCorrection cc = new ContrastCorrection(50);
            Invert invert = new Invert();

            ColorFiltering cor = new ColorFiltering
            {
                Red = new AForge.IntRange(155, 255),
                Green = new AForge.IntRange(155, 255),
                Blue = new AForge.IntRange(155, 255)
            };

            BlobsFiltering bc = new BlobsFiltering
            {
                CoupledSizeFiltering = false,
                MinHeight = 25,
                MaxHeight = 35
            };

            FiltersSequence seq = new FiltersSequence(gs, cc, cor, bc, invert);
            //FiltersSequence seq = new FiltersSequence(gs, invert, open, invert, bc, inverter, open, cc, cor, bc, inverter);

            image = seq.Apply(image);

            return image;
        }

        private string OCRImage(Bitmap image)
        {
            image = CropImage(image);
            image = ResizeImageSlow(image, 800, 180);
            image = CleanImage(image);
            return OCR3(image);
        }

        private string OCR2(Bitmap img)
        {
            string result = "";
            using (TesseractEngine engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                Page page = engine.Process(img, PageSegMode.Auto);
                result = page.GetText();
            }
            return result;
        }

        private string OCR3(Bitmap img)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                // Whitelist of chars to recognize
                engine.SetVariable("tessedit_char_whitelist", "ACDEIKLMNOTW");
                //engine.SetVariable("tessedit_char_whitelist", "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                // Dont bother with word plausibility
                engine.SetVariable("tessedit_unrej_any_wd", true);

                // Look for one line
                //using (var page = engine.Process(b, PageSegMode.SingleLine))

                Page page = engine.Process(img, PageSegMode.Auto);
                res = page.GetText();
            }
            return res;
        }

        private string ReconhecerCaptcha(Image img)
        {
            Bitmap imagem = new Bitmap(img);
            imagem = imagem.Clone(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //Erosion erosion = new Erosion();
            //Dilatation dilatation = new Dilatation();
            Invert inverter = new Invert();
            ColorFiltering cor = new ColorFiltering
            {
                Blue = new AForge.IntRange(200, 255),
                Red = new AForge.IntRange(200, 255),
                Green = new AForge.IntRange(200, 255)
            };
            Opening open = new Opening();
            BlobsFiltering bc = new BlobsFiltering
            {
                MinHeight = 10
            };
            //Closing close = new Closing();
            GaussianSharpen gs = new GaussianSharpen();
            ContrastCorrection cc = new ContrastCorrection();
            FiltersSequence seq = new FiltersSequence(gs, inverter, open, inverter, bc, inverter, open, cc, cor, bc, inverter);

            Image image = seq.Apply(imagem);
            string reconhecido = OCR((Bitmap)image);

            //image.Save("C:/img/processedImage.png", System.Drawing.Imaging.ImageFormat.Png);

            imagem.Dispose();

            return reconhecido;
        }

        private string OCR(Bitmap img)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                // Whitelist of chars to recognize
                engine.SetVariable("tessedit_char_whitelist", "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                // Dont bother with word plausibility
                engine.SetVariable("tessedit_unrej_any_wd", true);

                // Look for one line
                //using (var page = engine.Process(b, PageSegMode.SingleLine))

                // Look for all possible text
                using (var page = engine.Process(img, PageSegMode.SparseText))
                {
                    res = page.GetText();
                }

            }
            return res;
        }

        private Bitmap ResizeImageDouble(Image image)
        {
            Bitmap newImage = new Bitmap(image.Width * 2, image.Height * 2);
            using (Graphics g1 = Graphics.FromImage(newImage))
            {
                g1.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g1.DrawImage(image, 0, 0, image.Width * 2, image.Height * 2);
            }
            return newImage;
        }

        private Bitmap ResizeImageSlow(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private Bitmap ResizeImageQuick(Image image, int width, int height)
        {
            return new Bitmap(image, new Size(width, height));
        }

    }
}
