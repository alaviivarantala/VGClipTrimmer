using System;
using System.Threading;
using System.Threading.Tasks;
using VGClipTrimmer.Helpers;
using VGClipTrimmer.MVVM.Models.Interfaces;

namespace VGClipTrimmer.MVVM.Models.Services
{
    public class VideoProcessingService : IVideoProcessingService
    {
        public VideoFileInfo GetVideoFileInfo(string pathToVideo)
        {
            VideoFileInfo videoFile = new VideoFileInfo();


            return new VideoFileInfo();
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
