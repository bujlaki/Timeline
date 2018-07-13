using System;
using Xamarin.Forms;

using Timeline.Models;
using Timeline.Objects.Timeline;

namespace Timeline.ViewModels
{
	public class VMTimeline : Base.VMBase
    {
        private MTimeline _timeline;

        public Command CmdLongTap { get; set; }
		public MTimeline Timeline { get { return _timeline; } set { _timeline = value; } }

		public VMTimeline() : base()
        {
            CmdLongTap = new Command(LongTapExecute);
			Timeline = new MTimeline();

            for (int y = 2018; y < 2025; y++)
            {
                for (int i = 1; i < 11; i++)
                {
                    Timeline.Events.Add(new MTimelineEvent("event1 with long title", new TimelineDateTime(y, i), 2));
                }
            }

            EventManager.SortEventsToLanes(ref _timeline, 10);
        }

        private void LongTapExecute(object obj)
        {
            LongTapEventArg arg = (LongTapEventArg)obj;
            
            Acr.UserDialogs.UserDialogs.Instance.Alert("longtap " + arg.X.ToString() + " : " + arg.Y.ToString() + " : " + arg.Lane.ToString());
        }
    }
}
