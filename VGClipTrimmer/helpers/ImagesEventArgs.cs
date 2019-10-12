using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGClipTrimmer.helpers
{
    public class ImagesEventArgs : EventArgs
    {
        public int Seconds { get; set; }
        public byte[] Image { get; set; }
        public ImagesEventArgs(int seconds, byte[] image)
        {
            Seconds = seconds;
            Image = image;
        }
    }
}
