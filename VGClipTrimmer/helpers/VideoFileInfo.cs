using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGClipTrimmer.Helpers
{
    public class VideoFileInfo
    {
        public string MD5Sum { get; set; }
        public TimeSpan VideoLength { get; set; }
        public TimeSpan Processed { get; set; }

        public VideoFileInfo()
        {
        }
    }
}
