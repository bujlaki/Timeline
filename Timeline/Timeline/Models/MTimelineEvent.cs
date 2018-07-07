using System;
using Xamarin.Forms;

using Timeline.Objects.Date;

namespace Timeline.Models
{
    public class MTimelineEvent
    {
		private TimelineDateTime startDate;
		private TimelineDateTime endDate;

        public string Title { get; set; }

        public Uri Link { get; set; }

        public Image Image { get; set; }

		public TimelineDateTime StartDate { 
			get { return startDate; } 
			set { startDate = value; } 
		}

		public TimelineDateTime EndDate { 
			get { return endDate; } 
			set { endDate = value; }
		}
        
		public MTimelineEvent(string _title, TimelineDateTime _startdate, int _length=1)
        {
			Title = _title;
			startDate = _startdate;
			endDate = new TimelineDateTime(DateTime.UtcNow);
			startDate.CopyTo(ref endDate);
			endDate.Add(_length);
        }

    }
}
