using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameHighlightClipper.MVVM.Models.Interfaces
{
    public interface IVideoProcessingService
    {
        VideoFile GetVideoFileInfo(string pathToVideo);

        List<TimeSpan> ProcessVideoFile(VideoFile videoFile, IProgress<int> progress, CancellationToken token);
        List<TimeSpan> ProcessVideoFileYield(VideoFile videoFile, IProgress<int> progress, CancellationToken token);
    }
}