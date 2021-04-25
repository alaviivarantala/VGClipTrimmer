using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameHighlightClipper.Helpers;

namespace GameHighlightClipper.MVVM.Models.Interfaces
{
    public interface IVideoProcessingService
    {
        VideoFile GetVideoFileInfo(string pathToVideo);
        Task<object> ProcessVideoFile(string pathToVideo, IProgress<int> progress, CancellationToken token);
    }
}
