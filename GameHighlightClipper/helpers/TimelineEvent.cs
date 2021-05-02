using GalaSoft.MvvmLight;
using System;

namespace GameHighlightClipper.Helpers
{
    public class TimelineEvent : ObservableObject
    {
        private TimeSpan _start;
        public TimeSpan Start
        {
            get => _start;
            set => Set(ref _start, value);
        }


        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get => _duration;
            set => Set(ref _duration, value);
        }

    }
}
