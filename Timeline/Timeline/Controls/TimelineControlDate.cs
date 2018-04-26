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
            Day = 1;
            Month = 1;
            Year = 1;
            Decade = 1;
            Century = 1;
            KYear = 1;
            KKYear = 1;
            KKKYear = 1;
            MYear = 1;
        }
    }
}
