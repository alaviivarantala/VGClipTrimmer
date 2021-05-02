using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;

namespace GameHighlightClipper.Helpers
{
    public class Timeline : ObservableObject
    {
        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get => _duration;
            set => Set(ref _duration, value);
        }


        private ObservableCollection<TimelineEvent> _events = new ObservableCollection<TimelineEvent>();
        public ObservableCollection<TimelineEvent> Events
        {
            get => _events;
            set => Set(ref _events, value);
        }
    }
}
