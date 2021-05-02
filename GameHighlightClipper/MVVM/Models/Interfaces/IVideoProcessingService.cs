using System;
using System.Collections.Generic;
using System.Threading;

namespace GameHighlightClipper.MVVM.Models.Interfaces
{
    public interface IVideoProcessingService
    {
        VideoFile GetVideoFileInfo(VideoFile videoFile);

        List<TimeSpan> ProcessVideoFile(VideoFile videoFile, IProgress<int> progress, CancellationToken token);
        List<TimeSpan> ProcessVideoFileYield(VideoFile videoFile, IProgress<int> progress, CancellationToken token);
    }
}