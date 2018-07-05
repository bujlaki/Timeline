using System;

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

    class TLDate : BCACDateTime
    {
        private const TLDatePrecision PRECISION_MIN = TLDatePrecision.Minute;
        private const TLDatePrecision PRECISION_MAX = TLDatePrecision.BYear;
        private const long MAX_YEAR = 99000000000;

        private Int64 exactYear;
        private TLDatePrecision precision;

        public bool AD { get; set; }
        public Int64 TotalSeconds { get; set; }

        public Int64 Year
        {
            get
            {
                return exactYear;
            }
            set
            {
                exactYear = value;
                bcac = exactYear < 0 ? BCAC.BC : BCAC.AC;
            }
        }
        public TLDate(string yearstr)
        {
            if (yearstr.ToLower().EndsWith("b"))
            {
                //billion years
                yearstr = yearstr.ToLower().Replace("b", "");
                precision = TLDatePrecision.BYear;
                bcacDate = null;

                Year = int.Parse(yearstr) * MultiplierForPrecision(precision);
                if (Math.Abs(exactYear) > MAX_YEAR)
                {
                    exactYear = 0;
                    throw new OverflowException("Maximum 99 billion years");
                }
            }
            else if (yearstr.ToLower().EndsWith("m"))
            {
                //million years
                yearstr = yearstr.ToLower().Replace("m", "");
                precision = TLDatePrecision.MYear;
                bcacDate = null;

                while (yearstr.EndsWith("0") && precision < PRECISION_MAX) {
                    precision += 1;
                    yearstr = yearstr.Substring(0, yearstr.Length - 1);
                    if (String.IsNullOrEmpty(yearstr)) throw new ArgumentException("Incorrect value");
                }

                Year = int.Parse(yearstr) * MultiplierForPrecision(precision);
                if (Math.Abs(exactYear) > MAX_YEAR)
                {
                    exactYear = 0;
                    throw new OverflowException("Maximum 99 billion years");
                }
            }
            else if (yearstr.ToLower().EndsWith("k"))
            {
                //thousand years
                yearstr = yearstr.ToLower().Replace("k", ""); 
                precision = TLDatePrecision.KYear;
                bcacDate = null;

                while (yearstr.EndsWith("0") && precision < PRECISION_MAX)
                {
                    precision += 1;
                    yearstr = yearstr.Substring(0, yearstr.Length - 1);
                    if (String.IsNullOrEmpty(yearstr)) throw new ArgumentException("Incorrect value");
                }

                Year = long.Parse(yearstr) * MultiplierForPrecision(precision);
                if (Math.Abs(exactYear) > MAX_YEAR)
                {
                    exactYear = 0;
                    throw new OverflowException("Maximum 99 billion years");
                }

                if (exactYear < 10000)
                {
                    initBCACDate((int)exactYear, -1, -1, -1, -1, false);
                }
            }

        }

        public TLDate(int year, int month = -1, int day = -1, int hour = -1, int minute = -1)
        {
            initBCACDate(year, month, day, hour, minute);
        }

        public TLDate(Int64 year)
        {
            if(Math.Abs(year)<10000)
            {
                initBCACDate((int)year);
            }
            else
            {
                bcacDate = null;
                precision = TLDatePrecision.Year;
                if (year<0) {
                    exactYear = -year;
                    bcac = BCAC.BC;
                }
                else {
                    exactYear = year;
                    bcac = BCAC.AC;
                }
            }
        }

        private void initBCACDate(int year, int month = -1, int day = -1, int hour = -1, int minute = -1, bool setPrecision = true)
        {
            year = year % 10000;

            if (month == -1)
            {
                bcacDate = new DateTime(year, 1, 1);
                if(setPrecision) precision = TLDatePrecision.Year;
            }
            else if (day == -1)
            {
                bcacDate = new DateTime(year, month, 1);
                if (setPrecision) precision = TLDatePrecision.Month;
            }
            else if (hour == -1)
            {
                bcacDate = new DateTime(year, month, day);
                if (setPrecision) precision = TLDatePrecision.Day;
            }
            else if (minute == -1)
            {
                bcacDate = new DateTime(year, month, day, hour, 0, 0);
                if (setPrecision) precision = TLDatePrecision.Hour;
            }
            else
            {
                bcacDate = new DateTime(year, month, day, hour, minute, 0);
                if (setPrecision) precision = TLDatePrecision.Minute;
            }
        }

        private long MultiplierForPrecision(TLDatePrecision precision)
        {
            switch (precision)
            {
                case TLDatePrecision.KKYear: return 10000;
                case TLDatePrecision.KKKYear: return 100000;
                case TLDatePrecision.MYear: return 1000000;
                case TLDatePrecision.KMYear: return 10000000;
                case TLDatePrecision.KKMYear: return 100000000;
                case TLDatePrecision.BYear: return 1000000000;
                default: return 0;
            }
        }

        protected override void BCACDateChanged(DateChangedArgs args)
        {
            
            base.BCACDateChanged(args);
        }
    }
}
