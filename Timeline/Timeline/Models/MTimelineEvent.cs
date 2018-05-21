using System;
using Xamarin.Forms;

using Timeline.Controls;

namespace Timeline.Models
{
    public class MTimelineEvent
    {
		private MTimelineDate startDate;
		private MTimelineDate endDate;

        public string Title { get; set; }

        public Uri Link { get; set; }

        public Image Image { get; set; }

		public MTimelineDate StartDate { 
			get { return startDate; } 
			set { startDate = value; } 
		}

		public MTimelineDate EndDate { 
			get { return endDate; } 
			set { endDate = value; }
		}
        
		public MTimelineEvent(string _title, MTimelineDate _startdate, int _length=1)
        {
			Title = _title;
			startDate = _startdate;
			endDate = new MTimelineDate(DateTime.UtcNow);
			startDate.CopyTo(ref endDate);
			endDate.Add(_length);
        }

    }
}
