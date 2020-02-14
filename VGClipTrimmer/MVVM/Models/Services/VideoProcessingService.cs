using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VGClipTrimmer.MVVM.Models.Interfaces;

namespace VGClipTrimmer.MVVM.Models.Services
{
    public class VideoProcessingService : IVideoProcessingService
    {
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
