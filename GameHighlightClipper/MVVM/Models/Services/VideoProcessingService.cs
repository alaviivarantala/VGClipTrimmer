using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models.Interfaces;
using GameHighlightClipper.Video.Processing;
using Tesseract;

namespace GameHighlightClipper.MVVM.Models.Services
{
    public class VideoProcessingService : IVideoProcessingService
    {
        public VideoFile GetVideoFileInfo(string pathToVideo)
        {
            VideoFile videoFile = new VideoFile
            {
                MD5Sum = FileTools.CalculateMD5(pathToVideo),
                FileName = new FileInfo(pathToVideo).Name,
                FilePath = pathToVideo,
                FileSize = FileTools.GetFileSize(pathToVideo),
                Processed = 0
            };

            // ffmpeg probe output in one string
            string ffmpegProbeOut = FFmpeg.Info(pathToVideo);

            // we convert returns (\r) to newlines (\n) for easier time splitting the string
            // each line contains key=value
            string[] probeLines = ffmpegProbeOut.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            // grab video width and height from probe output
            videoFile.Width = int.Parse(probeLines.Where(x => x.Contains("width=")).FirstOrDefault().Split('=')[1]);
            videoFile.Height = int.Parse(probeLines.Where(x => x.Contains("height=")).FirstOrDefault().Split('=')[1]);

            // framerate can be a whole number (60) or a division (60000/1001, comes out at around 59.94) 
            string frameRate = probeLines.Where(x => x.Contains("r_frame_rate=")).FirstOrDefault().Split('=')[1];
            if (frameRate.Contains("/"))
            {
                videoFile.FrameRate = double.Parse(frameRate.Split('/')[0]) / double.Parse(frameRate.Split('/')[1]);
            }
            else
            {
                videoFile.FrameRate = double.Parse(frameRate);
            }

            // we save the video duration in seconds
            string[] duration = probeLines.Where(x => x.Contains("duration=")).FirstOrDefault().Split('=')[1].Split('.')[0].Split(':');
            videoFile.VideoLength = (int)new TimeSpan(int.Parse(duration[0]), int.Parse(duration[1]), int.Parse(duration[2])).TotalSeconds;

            return videoFile;
        }

        public List<TimeSpan> ProcessVideoFileYield(VideoFile videoFile, IProgress<int> progress, CancellationToken token)
        {
            int ww = videoFile.Width;
            int hh = videoFile.Height;
            int length = videoFile.VideoLength;

            double widthMultiplier = 0.25;
            double heightMultiplier = 0.05;
            int width = (int)(ww * widthMultiplier / 1.75);
            int height = (int)(hh * heightMultiplier);
            int startingPointX = (int)((ww / 2) - (width / 1.4));
            int startingPointY = (int)((hh * 0.75) - (height / 0.9));

            List<Tuple<int, bool>> tuples = new List<Tuple<int, bool>>();

            var images = FFmpeg.SnapshotEverySecondYield(videoFile.FilePath, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString(), 307200);
            /*
            foreach (var image in images)
            {
                tuples.Add(OCRImage(tuples.Count + 1, image));
                progress.Report(1);
            }
            */

            int maxDegreeOfParallelism = Environment.ProcessorCount;
            /*
            Parallel.For(0, images.Count, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                var image = images.ElementAt(i);
                tuples.Add(OCRImage(i, image));
                progress.Report(1);
            });
            */
            Parallel.ForEach(images, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                tuples.Add(OCRImage(tuples.Count, i));
                progress.Report(1);
            });

            List<TimeSpan> results = tuples.Where(result => result.Item2).Select(time => TimeSpan.FromSeconds(time.Item1)).ToList();
            results.Sort();

            return results;
        }

        public List<TimeSpan> ProcessVideoFile(VideoFile videoFile, IProgress<int> progress, CancellationToken token)
        {
            int ww = videoFile.Width;
            int hh = videoFile.Height;
            int length = videoFile.VideoLength;

            double widthMultiplier = 0.25;
            double heightMultiplier = 0.05;
            int width = (int)(ww * widthMultiplier / 1.75);
            int height = (int)(hh * heightMultiplier);
            int startingPointX = (int)((ww / 2) - (width / 1.4));
            int startingPointY = (int)((hh * 0.75) - (height / 0.9));

            List<Tuple<int, bool>> tuples = new List<Tuple<int, bool>>();

            List<byte[]> images = FFmpeg.SnapshotEverySecond(videoFile.FilePath, width.ToString(), height.ToString(), startingPointX.ToString(), startingPointY.ToString(), 307200);

            int maxDegreeOfParallelism = Environment.ProcessorCount;
            Parallel.For(0, images.Count, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, (i) =>
            {
                tuples.Add(OCRImage(i, images[i]));
                progress.Report(1);
            });

            List<TimeSpan> results = tuples.Where(result => result.Item2).Select(time => TimeSpan.FromSeconds(time.Item1)).ToList();
            results.Sort();

            return results;
        }

        private Tuple<int, bool> OCRImage(int seconds, byte[] imageBytes)
        {
            bool result = false;
            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                using (Bitmap image = new Bitmap(memoryStream))
                {
                    using (Bitmap imageEdit = ResizeCleanInvertImage(image))
                    {
                        string resultOCR = OCR(imageEdit);
                        if (resultOCR.ToLower().Contains("eliminated") || resultOCR.ToLower().Contains("knocked"))
                        {
                            TimeSpan time = TimeSpan.FromSeconds(seconds);
                            result = true;
                        }
                    }
                }
            }
            return new Tuple<int, bool>(seconds, result);
        }

        private Bitmap ResizeCleanInvertImage(Bitmap image)
        {
            image = ImageEditing.ResizeImageSlow(image, 800, 158);
            image = ImageEditing.CleanImage(image);
            return image;
        }

        private string OCR(Bitmap img)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"./video/tessdata", "eng", EngineMode.Default))
            {
                // whitelist of chars to recognize
                engine.SetVariable("tessedit_char_whitelist", "ACDEIKLMNOTW");
                // dont bother with word plausibility
                engine.SetVariable("tessedit_unrej_any_wd", true);

                Page page = engine.Process(img, PageSegMode.Auto);
                res = page.GetText();
            }
            return res;
        }
    }
}
