using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.Models.Interfaces;

namespace GameHighlightClipper.MVVM.Models.Services
{
    public class VideoProcessingService : IVideoProcessingService
    {
        public VideoFileInfo GetVideoFileInfo(string pathToVideo)
        {
            VideoFileInfo videoFile = new VideoFileInfo();

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

        public async Task<object> ProcessVideoFile(string video, IProgress<int> progress, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                break;
            }

            return null;
        }
    }
}
