using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGClipTrimmer.helpers
{
    public class Settings
    {
        public bool PlaceholderSetting { get; set; }

        public Settings(bool placeholderSetting)
        {
            PlaceholderSetting = placeholderSetting;
        }
    }
}
