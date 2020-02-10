using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGClipTrimmer.helpers
{
    public class Settings
    {
        // dark or light
        public bool Theme { get; set; }

        // time to include before and after a highlight
        public int GracePeriod { get; set; }

        public Settings()
        {

        }
    }
}
