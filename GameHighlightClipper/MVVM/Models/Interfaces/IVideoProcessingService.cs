using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameHighlightClipper.MVVM.Models.Interfaces
{
    public interface IVideoProcessingService
    {
        Task<VideoFile> GetVideoFileInfo(VideoFile videoFile);

        Task<List<TimeSpan>> ProcessVideoFile(VideoFile videoFile, IProgress<int> progress, CancellationToken token);
    }
}