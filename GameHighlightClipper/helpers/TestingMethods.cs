using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Tesseract;
using GameHighlightClipper.Video.Processing;

namespace GameHighlightClipper.Helpers
{
    public static class TestingMethods
    {

        public static async void PHM()
        {
            string clips = "C:/clips/";
            await Task.Run(() => TestBatch(clips));
            //var r = FFmpeg.CutVideo(clips + "APEX2.mp4", clips + "outAPEX2.mp4", new TimeSpan(0, 0, 55), new TimeSpan(0, 3, 55));
            //TestingMethods.CreateImages(clips);
            General.ShutdownApp();
        }

        public static void CreateImages(string clips)
        {
            string video = clips + "APEX2.mp4";
            string[] lines = FFmpeg.Info(video).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            int ww = int.Parse(lines.Where(x => x.Contains("width=")).FirstOrDefault().Split('=')[1]);
            int hh = int.Parse(lines.Where(x => x.Contains("height=")).FirstOrDefault().Split('=')[1]);

            string[] duration = lines.Where(x => x.Contains("duration=")).FirstOrDefault().Split('=')[1].Split('.')[0].Split(':');
            int length = (int)new TimeSpan(int.Parse(duration[0]), int.Parse(duration[1]), int.Parse(duration[2])).TotalSeconds;

            int frames = 3;
            int fps = (int)Math.Round((double)length / frames);

            List<List<Bitmap>> cropped = new List<List<Bitmap>>();

            int ww2 = ww / 2;
            int hh2 = hh / 2;

            List<Tuple<int, int>> positions = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0, 0),
                new Tuple<int, int>(ww2, 0),
                new Tuple<int, int>(0, hh2),
                new Tuple<int, int>(ww2, hh2)
            };

            for (int i = 0; i < frames; i++)
            {
                cropped.Add(new List<Bitmap>()); ;
            }

            int maxDegreeOfParallelism = Environment.ProcessorCount;
            Parallel.For(0, frames, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                TimeSpan span = new TimeSpan(0, 0, fps * i);
                Bitmap image = FFmpeg.Snapshot(span, video);
                for (int j = 0; j < 4; j++)
                {
                    cropped[i].Add(ImageEditing.CropImage(image, positions[j].Item1, positions[j].Item2));
                }
            });

            List<Tuple<GameEnum, string>> uiImages = new List<Tuple<GameEnum, string>>
            {
                new Tuple<GameEnum, string>(GameEnum.APEX, clips + "APEXUIGS.png"),
                new Tuple<GameEnum, string>(GameEnum.PUBG, clips + "PUBGUIGS.png")
            };

            List<Tuple<GameEnum, List<Bitmap>>> slicedUiImages = new List<Tuple<GameEnum, List<Bitmap>>>();

            for (int i = 0; i < uiImages.Count; i++)
            {
                slicedUiImages.Add(new Tuple<GameEnum, List<Bitmap>>(uiImages[i].Item1, new List<Bitmap>()));
                Bitmap image = (Bitmap)Image.FromFile(uiImages[i].Item2);
                image = ImageEditing.ResizeImageSlow(image, ww, hh);
                for (int j = 0; j < 4; j++)
                {
                    slicedUiImages[i].Item2.Add(ImageEditing.CropImage(image, positions[j].Item1, positions[j].Item2));
                }
            }

            List<Tuple<GameEnum, List<float>>> comparsions = new List<Tuple<GameEnum, List<float>>>();

            // games
            for (int i = 0; i < slicedUiImages.Count; i++)
            {
                Tuple<GameEnum, List<Bitmap>> uiSlices = slicedUiImages[i];

                List<float> values = new List<float>();

                // frames
                for (int j = 0; j < cropped.Count; j++)
                {
                    List<Bitmap> frameSlices = cropped[j];

                    // slices
                    for (int k = 0; k < 4; k++)
                    {
                        values.Add(100 - ImageComparsion.GetBhattacharyyaDifference(frameSlices[k], uiSlices.Item2[k]) * 100);
                        SaveImage(clips, frameSlices[k], "frame" + i.ToString() + j.ToString() + k.ToString());
                        SaveImage(clips, uiSlices.Item2[k], "ui" + i.ToString() + j.ToString() + k.ToString());
                    }
                }
                comparsions.Add(new Tuple<GameEnum, List<float>>(slicedUiImages[i].Item1, values));
            }

            float avg1 = comparsions[0].Item2.Where(x => !float.IsNaN(x)).OrderByDescending(x => x).Take(3).Average();
            float avg2 = comparsions[1].Item2.Where(x => !float.IsNaN(x)).OrderByDescending(x => x).Take(3).Average();
        }

        public static void TestBatch(string clips)
        {
            string video = clips + "APEX3.mp4";

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

            List<byte[]> images = FFmpeg.SnapshotEverySecond(video, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString(), 307200);

            int maxDegreeOfParallelism = Environment.ProcessorCount;
            Parallel.For(0, images.Count, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                tuples.Add(OCRImage(i, images[i]));
            });

            List<TimeSpan> results = tuples.Where(result => result.Item2).Select(time => TimeSpan.FromSeconds(time.Item1)).ToList();
        }

        public static Tuple<int, bool> OCRImage(int seconds, byte[] imageBytes)
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

        public static string TestOCRImage(Bitmap image)
        {
            return OCR(ResizeCleanImage(image));
        }

        public static Bitmap ResizeCleanImage(Bitmap image)
        {
            image = ImageEditing.ResizeImageSlow(image, 800, 158);
            image = ImageEditing.CleanImage(image);
            return image;
        }

        public static string OCR(Bitmap img)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"./video/tessdata", "eng", EngineMode.Default))
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

        public static void SaveImage(string clips, Image image, string name)
        {
            using (Bitmap temp = new Bitmap(image))
            {
                temp.Save(clips + "tempb/" + name + ".png");
            }
        }
    }
}
