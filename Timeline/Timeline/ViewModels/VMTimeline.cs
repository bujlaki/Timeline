using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Essentials;
using Acr.UserDialogs;

using Timeline.Models;
using Timeline.Objects.Timeline;


namespace Timeline.ViewModels
{
	public class VMTimeline : Base.VMBase
    {
        private TimelineUnits zoomUnit;
        public TimelineUnits ZoomUnit
        {
            get { return zoomUnit; }
            set { zoomUnit = value; RaisePropertyChanged("ZoomUnit"); }
        }

        private TimelineDateTime date;
        public TimelineDateTime Date
        {
            get { return date; }
            set { date = value; RaisePropertyChanged("Date"); }
        }

        public Int64 Pixeltime { get; }

        public Command CmdLongTap { get; set; }
        public ObservableCollection<MTimelineEvent> Events { get; set; }
        public int LaneCount { get; set; }

		public VMTimeline() : base()
        {
            CmdLongTap = new Command(LongTapExecute);
            Events = new ObservableCollection<MTimelineEvent>();
        }

        private void LongTapExecute(object obj)
        {
            LongTapEventArg arg = (LongTapEventArg)obj;

            //new event
            TimelineDateTime tld = TimelineDateTime.FromTicks(arg.Ticks);
            tld.Precision = arg.ZoomUnit;
            
            MainThread.BeginInvokeOnMainThread(() => App.services.Navigation.GoToTimelineEventView(new MTimelineEvent("new event", tld)));
        }

        public void LoadEvents(string timelineId)
        {
            Task.Run(async () => {
                Events = new ObservableCollection<MTimelineEvent>(await App.services.Database.GetEvents(timelineId));
                LaneCount = EventManager.SortEventsToLanes(Events, 10);
            });
        }
    }
}
