using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Objects.Date
{
    //DateTime ticks: 
    //MIN : 0
    //MAX : 3155378975999999999
    //1 min : 600.000.000 tick
    //max minutes (10000 years): 3,155,378,976,000,000,000 / 600,000,000 = 5,258,964,960
    public enum TLDatePrecision
    {
        Minute,
        Hour,
        Day,
        Month,
        Year,
        KYear,
        KKYear,
        KKKYear,
        MYear,
        KMYear,
        KKMYear,
        BYear
    }

    class TLDate : RollingDateTime
    {
        
        private long kkyear;
        private TLDatePrecision precision;

        public bool AD { get; set; }
        public Int64 TotalSeconds { get; set; }

        public TLDate(string yearstr)
        {
            if (yearstr.ToLower().EndsWith("b"))
            {
                yearstr = yearstr.ToLower().Replace("b", "");
                precision = TLDatePrecision.BYear;
                kkyear = int.Parse(yearstr) * 100000;
            }
            else if (yearstr.ToLower().EndsWith("kkm"))
            {
                yearstr = yearstr.ToLower().Replace("kkm", "");
                precision = TLDatePrecision.KKMYear;
                kkyear = int.Parse(yearstr) * 10000;
            }
            else if (yearstr.ToLower().EndsWith("km"))
            {
                yearstr = yearstr.ToLower().Replace("km", "");
                precision = TLDatePrecision.KMYear;
                kkyear = int.Parse(yearstr) * 1000;
            }
            else if (yearstr.ToLower().EndsWith("m"))
            {
                yearstr = yearstr.ToLower().Replace("m", "");
                precision = TLDatePrecision.MYear;
                kkyear = int.Parse(yearstr) * 100;
            }
            else if (yearstr.ToLower().EndsWith("kkk"))
            {
                yearstr = yearstr.ToLower().Replace("kkk", "");
                precision = TLDatePrecision.KKKYear;
                kkyear = int.Parse(yearstr) * 10;
            }
            else if (yearstr.ToLower().EndsWith("kk"))
            {
                yearstr = yearstr.ToLower().Replace("kk", "");
                precision = TLDatePrecision.KKYear;
                kkyear = int.Parse(yearstr);
            }
        }

        public TLDate(long year, int month = -1, int day = -1, int hour = -1, int minute = -1)
        {
            kkyear = year / 10000;
            year -= (kkyear * 10000);

            if (month == -1)
            {
                Value = new DateTime((int)year, 1, 1);
                precision = TLDatePrecision.Year;
            }
            else if (day == -1)
            {
                Value = new DateTime((int)year, month, 1);
                precision = TLDatePrecision.Month;
            }
            else if (hour == -1)
            {
                Value = new DateTime((int)year, month, day);
                precision = TLDatePrecision.Day;
            }
            else if (minute == -1)
            {
                Value = new DateTime((int)year, month, day, hour, 0, 0);
                precision = TLDatePrecision.Hour;
            }
            else
            {
                Value = new DateTime((int)year, month, day, hour, minute, 0);
                precision = TLDatePrecision.Minute;
            }
        }

        public TLDate(long kkyear, DateTime dateTime)
        {
        }

        protected override void RollOver(int count)
        {
            base.RollOver(count);
        }

        protected override void DateChanged()
        {
            base.DateChanged();


        }
    }
}
