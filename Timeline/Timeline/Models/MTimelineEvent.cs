using System;
using Xamarin.Forms;

using Timeline.Objects.Date;

namespace Timeline.Models
{
    public class MTimelineEvent
    {
        public string Title { get; set; }
        public Uri Link { get; set; }
        public Image Image { get; set; }
		public TimelineDateTime StartDate { get; set; }
		public TimelineDateTime EndDate { get; set; }
        public int LaneNumber { get; set; }

		public MTimelineEvent(string _title, TimelineDateTime _startdate, int _length=1)
        {
            TimelineDateTime tempDate = new TimelineDateTime();

			Title = _title;
			StartDate = _startdate;
			StartDate.CopyTo(ref tempDate);
            tempDate.Add(_length);
            EndDate = tempDate;
        }

    }
}
