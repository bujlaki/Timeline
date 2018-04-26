using System;
namespace Timeline.Controls
{
    public class TimelineControlDate
    {
        private int minute;
        private int hour;
        private int day;
        private int month;
        private int year;
        private int decade;
        private int century;
        private int kyear;
        private int kkyear;
        private int kkkyear;
        private int myear;

        public int Minute { get; set; }
        public int Hour { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Decade { get; set; }
        public int Century { get; set; }
        public int KYear { get; set; }
        public int KKYear { get; set; }
        public int KKKYear { get; set; }
        public int MYear { get; set; }

        public TimelineControlDate()
        {
            Minute = 0;
            Hour = 0;
            Day = 0;
            Month = 0;
            Year = 0;
            Decade = 0;
            Century = 0;
            KYear = 0;
            KKYear = 0;
            KKKYear = 0;
            MYear = 0;
        }

        public void Add(TimelineUnits unit, int value)
        {
            switch(unit)
            {
                
            }
        }
    }
}
