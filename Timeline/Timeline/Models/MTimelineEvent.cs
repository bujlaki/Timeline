using System;
using Xamarin.Forms;

using Timeline.Controls;

namespace Timeline.Models
{
    public class MTimelineEvent
    {
        public string Title { get; set; }

        public Uri Link { get; set; }

        public Image Image { get; set; }

		public TimelineDate StartDate { get; set; }

		public TimelineDate EndDate { get; set; }

		public MTimelineEvent(string _title, DateTime _date)
        {
			Title = _title;
			StartDate = new TimelineDate(_date);
			EndDate = new TimelineDate(_date);
        }

    }
}
