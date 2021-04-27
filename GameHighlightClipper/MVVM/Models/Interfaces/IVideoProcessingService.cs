using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameHighlightClipper.MVVM.Models.Interfaces
{
    public interface IVideoProcessingService
    {
        VideoFile GetVideoFileInfo(string pathToVideo);
        Task<object> ProcessVideoFile(string pathToVideo, IProgress<int> progress, CancellationToken token);
    }
}
