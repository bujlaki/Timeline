﻿using System;
using Xamarin.Forms;

using Timeline.Objects.Timeline;

namespace Timeline.Models
{
    public class MTimelineEvent
    {
        public string EventId { get; set; }
        public string TimelineId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageBase64 { get; set; }
        public string URL { get; set; }
        public string Data { get; set; }
        public Int64 StartDateTicks { get { return StartDate.Ticks; } }
        public Int64 EndDateTicks { get { return EndDate.Ticks; } }
        public byte Precision { get; set; }

        //internal
        public Image Image { get; set; }
        public TimelineDateTime StartDate { get; set; }
        public TimelineDateTime EndDate { get; set; }
        public bool EndDateSet { get; set; }
        public int LaneNumber { get; set; }

        public MTimelineEvent()
        {
            EventId = Guid.NewGuid().ToString();
            Image = new Image();
            ClearImage();
        }

		public MTimelineEvent(string _title, TimelineDateTime _startdate, int _length=1) : this()
        {
            TimelineDateTime tempDate = new TimelineDateTime();

			Title = _title;
			StartDate = _startdate;
			StartDate.CopyTo(ref tempDate);
            tempDate.Add(_length);
            EndDate = tempDate;
            EndDateSet = false;
        }

        public void ClearImage() {
            Image.Source = "noimage";
        }
    }
}
