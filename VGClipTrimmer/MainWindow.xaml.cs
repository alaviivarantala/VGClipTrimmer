using AForge.Imaging.Filters;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Tesseract;
using Xabe.FFmpeg;

namespace VGClipTrimmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        string path = "../../img/";

        public MainWindow()
        {
            InitializeComponent();



            //System.Windows.Application.Current.Shutdown();
        }

        private void TestBatch()
        {
            string result1 = TestOCR(path + "apex1.png");
            string result2 = TestOCR(path + "apex2.png");
            string result3 = TestOCR(path + "apex3.png");
            string result4 = TestOCR(path + "apex4.png");
            string result5 = TestOCR(path + "apex5.png");
            string result6 = TestOCR(path + "apex6.png");
            string result7 = TestOCR(path + "apex7.png");
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
            image = image.Clone(new Rectangle(0, 0, image.Width, image.Height), PixelFormat.Format24bppRgb);

            GaussianSharpen gs = new GaussianSharpen();
            ContrastCorrection cc = new ContrastCorrection(50);
            Invert invert = new Invert();

            ColorFiltering cor = new ColorFiltering();
            cor.Red = new AForge.IntRange(155, 255);
            cor.Green = new AForge.IntRange(155, 255);
            cor.Blue = new AForge.IntRange(155, 255);

            BlobsFiltering bc = new BlobsFiltering();
            bc.CoupledSizeFiltering = false;
            bc.MinHeight = 25;
            bc.MaxHeight = 35;

            FiltersSequence seq = new FiltersSequence(gs, cc, cor, bc, invert);
            //FiltersSequence seq = new FiltersSequence(gs, invert, open, invert, bc, inverter, open, cc, cor, bc, inverter);

            image = seq.Apply(image);

            return image;
        }

        private string OCR2(Bitmap img)
        {
            TesseractEngine engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default);
            Page page = engine.Process(img, PageSegMode.Auto);
            string result = page.GetText();
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

        private string reconhecerCaptcha(Image img)
        {
            Bitmap imagem = new Bitmap(img);
            imagem = imagem.Clone(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //Erosion erosion = new Erosion();
            //Dilatation dilatation = new Dilatation();
            Invert inverter = new Invert();
            ColorFiltering cor = new ColorFiltering();
            cor.Blue = new AForge.IntRange(200, 255);
            cor.Red = new AForge.IntRange(200, 255);
            cor.Green = new AForge.IntRange(200, 255);
            Opening open = new Opening();
            BlobsFiltering bc = new BlobsFiltering();
            bc.MinHeight = 10;
            //Closing close = new Closing();
            GaussianSharpen gs = new GaussianSharpen();
            ContrastCorrection cc = new ContrastCorrection();
            FiltersSequence seq = new FiltersSequence(gs, inverter, open, inverter, bc, inverter, open, cc, cor, bc, inverter);

            Image image = seq.Apply(imagem);
            string reconhecido = OCR((Bitmap)image);

            //image.Save("C:/img/processedImage.png", System.Drawing.Imaging.ImageFormat.Png);

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
