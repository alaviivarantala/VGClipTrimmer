using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

// Created in 2012 by Jakob Krarup (www.xnafan.net).
// Use, alter and redistribute this code freely,
// but please leave this comment :)

namespace VGClipTrimmer.Video.Processing
{
    public static class ImageComparsion
    {
        public static float GetPercentageDifference(Image img1, Image img2, byte threshold = 3)
        {
            return img1.PercentageDifference(img2, threshold);
        }

        public static float GetBhattacharyyaDifference(Image img1, Image img2)
        {
            return img1.BhattacharyyaDifference(img2);
        }

        /*
        public static float GetPercentageDifference(string image1Path, string image2Path, byte threshold = 3)
        {
            Image img1 = Image.FromFile(image1Path);
            Image img2 = Image.FromFile(image2Path);

            float difference = img1.PercentageDifference(img2, threshold);

            img1.Dispose();
            img2.Dispose();

            return difference;
        }

        public static float GetBhattacharyyaDifference(string image1Path, string image2Path)
        {
            Image img1 = Image.FromFile(image1Path);
            Image img2 = Image.FromFile(image2Path);

            float difference = img1.BhattacharyyaDifference(img2);

            img1.Dispose();
            img2.Dispose();

            return difference;
        }
        */
        //the colormatrix needed to grayscale an image
        //http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
        private static readonly ColorMatrix ColorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });

        private static float PercentageDifference(this Image img1, Image img2, byte threshold = 3)
        {
            byte[,] differences = img1.GetDifferences(img2);

            int diffPixels = 0;

            foreach (byte b in differences)
            {
                if (b > threshold) { diffPixels++; }
            }

            return diffPixels / 256f;
        }

        private static float BhattacharyyaDifference(this Image img1, Image img2)
        {
            byte[,] img1GrayscaleValues = img1.GetGrayScaleValues();
            byte[,] img2GrayscaleValues = img2.GetGrayScaleValues();

            var normalizedHistogram1 = new double[16, 16];
            var normalizedHistogram2 = new double[16, 16];

            double histSum1 = 0.0;
            double histSum2 = 0.0;

            foreach (var value in img1GrayscaleValues) { histSum1 += value; }
            foreach (var value in img2GrayscaleValues) { histSum2 += value; }


            for (int x = 0; x < img1GrayscaleValues.GetLength(0); x++)
            {
                for (int y = 0; y < img1GrayscaleValues.GetLength(1); y++)
                {
                    normalizedHistogram1[x, y] = (double)img1GrayscaleValues[x, y] / histSum1;
                }
            }
            for (int x = 0; x < img2GrayscaleValues.GetLength(0); x++)
            {
                for (int y = 0; y < img2GrayscaleValues.GetLength(1); y++)
                {
                    normalizedHistogram2[x, y] = (double)img2GrayscaleValues[x, y] / histSum2;
                }
            }

            double bCoefficient = 0.0;
            for (int x = 0; x < img2GrayscaleValues.GetLength(0); x++)
            {
                for (int y = 0; y < img2GrayscaleValues.GetLength(1); y++)
                {
                    double histSquared = normalizedHistogram1[x, y] * normalizedHistogram2[x, y];
                    bCoefficient += Math.Sqrt(histSquared);
                }
            }

            double dist1 = 1.0 - bCoefficient;
            dist1 = Math.Round(dist1, 8);
            double distance = Math.Sqrt(dist1);
            distance = Math.Round(distance, 8);
            return (float)distance;

        }

        private static byte[,] GetDifferences(this Image img1, Image img2)
        {
            Bitmap thisOne = (Bitmap)img1.Resize(16, 16).GetGrayScaleVersion();
            Bitmap theOtherOne = (Bitmap)img2.Resize(16, 16).GetGrayScaleVersion();
            byte[,] differences = new byte[16, 16];
            byte[,] firstGray = thisOne.GetGrayScaleValues();
            byte[,] secondGray = theOtherOne.GetGrayScaleValues();

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    differences[x, y] = (byte)Math.Abs(firstGray[x, y] - secondGray[x, y]);
                }
            }
            thisOne.Dispose();
            theOtherOne.Dispose();
            return differences;
        }

        private static byte[,] GetGrayScaleValues(this Image img)
        {
            using (Bitmap thisOne = (Bitmap)img.Resize(16, 16).GetGrayScaleVersion())
            {
                byte[,] grayScale = new byte[16, 16];

                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        grayScale[x, y] = (byte)Math.Abs(thisOne.GetPixel(x, y).R);
                    }
                }
                return grayScale;
            }
        }

        private static Image GetGrayScaleVersion(this Image original)
        {
            //http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                //create some image attributes
                ImageAttributes attributes = new ImageAttributes();

                //set the color matrix attribute
                attributes.SetColorMatrix(ColorMatrix);

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                   0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }
            return newBitmap;

        }

        private static Image Resize(this Image originalImage, int newWidth, int newHeight)
        {
            Image smallVersion = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(smallVersion))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return smallVersion;
        }
    }
}
