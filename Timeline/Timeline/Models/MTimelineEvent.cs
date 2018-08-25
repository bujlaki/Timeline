using System;
using Xamarin.Forms;

using Timeline.Objects.Timeline;
using System.Collections.Generic;

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
        public string EventType { get; set; }

        //internal
        public Image Image { get; set; }
        public TimelineDateTime StartDate { get; set; }
        public TimelineDateTime EndDate { get; set; }
        public bool EndDateSet { get; set; }
        public int LaneNumber { get; set; }

        public MTimelineEvent()
        {
            EventId = Guid.NewGuid().ToString();
            EventType = "Default";
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

        public MTimelineEvent Copy()
        {
            MTimelineEvent target = new MTimelineEvent();
            target.Title = Title;
            target.TimelineId = TimelineId;
            target.Description = Description;
            target.Data = Data;
            target.URL = URL;
            target.StartDate = StartDate.Copy();
            target.EndDate = EndDate.Copy();
            target.EndDateSet = EndDateSet;
            target.ImageBase64 = ImageBase64;
            target.Image = ImageFromBase64(target.ImageBase64);
            target.LaneNumber = LaneNumber;
            return target;
        }

        private static Xamarin.Forms.Image ImageFromBase64(string base64picture)
        {
            byte[] imageBytes = Convert.FromBase64String(base64picture);
            return new Xamarin.Forms.Image { Source = Xamarin.Forms.ImageSource.FromStream(() => new System.IO.MemoryStream(imageBytes)) };
        }
    }
}
