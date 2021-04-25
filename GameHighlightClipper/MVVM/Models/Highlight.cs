using System;

namespace GameHighlightClipper.MVVM.Models
{
    public class Highlight
    {
        public int Kills { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }

        public Highlight()
        {

        }
    }
}
