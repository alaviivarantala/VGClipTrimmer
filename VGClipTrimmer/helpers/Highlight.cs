using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGClipTrimmer.helpers
{
    public class Highlight
    {
        public int Kills { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }

        public Highlight(int kills, TimeSpan from, TimeSpan to)
        {
            Kills = kills;
            From = from;
            To = to;
        }
    }
}
