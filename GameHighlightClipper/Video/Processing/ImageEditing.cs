using AForge.Imaging.Filters;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace GameHighlightClipper.Video.Processing
{
    public class ImageEditing
    {
        public static Bitmap CropImage(Bitmap image, int startingX, int startingY)
        {
            int width = image.Width / 2;
            int height = image.Height / 2;

            Rectangle section = new Rectangle(new Point(startingX, startingY), new Size(width, height));
            Bitmap bitmap = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        public static Bitmap CropImage(Bitmap image)
        {
            int ww = image.Width;
            int hh = image.Height;

            double widthMultiplier = 0.25;
            double heightMultiplier = 0.05;
            int width = (int)(ww * widthMultiplier / 1.75);
            int height = (int)(hh * heightMultiplier);
            int startingPointX = (int)((ww / 2) - (width / 1.4));
            int startingPointY = (int)((hh * 0.75) - (height / 0.9));

            Rectangle section = new Rectangle(new Point(startingPointX, startingPointY), new Size(width, height));
            Bitmap bitmap = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        public static Bitmap CleanImage(Bitmap image)
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
                CoupledSizeFiltering = true,
                MinHeight = 45,
                MaxHeight = 55
            };

            FiltersSequence seq = new FiltersSequence(gs, cc, cor, bc, invert);
            //FiltersSequence seq = new FiltersSequence(gs, invert, open, invert, bc, inverter, open, cc, cor, bc, inverter);

            image = seq.Apply(image);

            return image;
        }

        public static Bitmap ResizeImageDouble(Image image)
        {
            Bitmap newImage = new Bitmap(image.Width * 2, image.Height * 2);
            using (Graphics g1 = Graphics.FromImage(newImage))
            {
                g1.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g1.DrawImage(image, 0, 0, image.Width * 2, image.Height * 2);
            }
            return newImage;
        }

        public static Bitmap ResizeImageSlow(Image image, int width, int height)
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

        public static Bitmap ResizeImageQuick(Image image, int width, int height)
        {
            return new Bitmap(image, new Size(width, height));
        }

        public static byte[] BitmapToBytes(Bitmap Bitmap)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                Bitmap.Save(ms, Bitmap.RawFormat);
                byte[] byteImage = new Byte[ms.Length];
                byteImage = ms.ToArray();
                return byteImage;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }
    }
}
