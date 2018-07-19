using System;
using Xamarin.Forms;

using Timeline.Models;
using Timeline.Objects.Timeline;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Timeline.ViewModels
{
	public class VMTimeline : Base.VMBase
    {

        public Command CmdLongTap { get; set; }
        public ObservableCollection<MTimelineEvent> Events { get; set; }
        public int LaneCount { get; set; }

		public VMTimeline() : base()
        {
            CmdLongTap = new Command(LongTapExecute);
            Events = new ObservableCollection<MTimelineEvent>();

            for (int y = 2018; y < 2025; y++)
            {
                for (int i = 1; i < 11; i++)
                {
                    Events.Add(new MTimelineEvent("event1 with long title", new TimelineDateTime(y, i), 2));
                }
            }

            LaneCount = EventManager.SortEventsToLanes(Events, 10);
        }

        private void LongTapExecute(object obj)
        {
            LongTapEventArg arg = (LongTapEventArg)obj;
            
            Acr.UserDialogs.UserDialogs.Instance.Alert("longtap " + arg.X.ToString() + " : " + arg.Y.ToString() + " : " + arg.Lane.ToString());
        }
    }
}
