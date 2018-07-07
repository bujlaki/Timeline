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

    public class TLDate : BCACDateTime
    {
        private const TLDatePrecision PRECISION_MIN = TLDatePrecision.Minute;
        private const TLDatePrecision PRECISION_MAX = TLDatePrecision.BYear;
        private const float MINUTES_PER_YEAR = 525949.20f;
        private const long MINUTES_PER_KKYEAR = 5258964959;
        private const long TICKS_PER_MINUTE = 600000000;
        private const long MAX_YEAR = 99000000000;

        private Int64 exactYear;
        private TLDatePrecision precision;

        public bool AD { get { return bcac == BCAC.AC; } }
        public Int64 TotalMinutes { get; private set; }

        public Int64 Year
        {
            get
            {
                if (bcacDate == null)
                {
                    return exactYear;
                }
                else
                {
                    if (bcac == BCAC.BC) return 10000 - bcacDate.Value.Year;
                    return bcacDate.Value.Year;
                }
            }
            set
            {
                exactYear = value;
                bcac = exactYear < 0 ? BCAC.BC : BCAC.AC;
            }
        }
        public string YearStr
        {
            get
            {
                return (bcac == BCAC.BC) ? "BC. " + Year.ToString() : "AC. " + Year.ToString();
            }
        }
        public string PrecisionStr
        {
            get
            {
                switch (precision)
                {
                    case TLDatePrecision.Minute: return "Minute";
                    case TLDatePrecision.Hour: return "Hour";
                    case TLDatePrecision.Day: return "Day";
                    case TLDatePrecision.Month: return "Month";
                    case TLDatePrecision.Year: return "Year";
                    case TLDatePrecision.KYear: return "KYear";
                    case TLDatePrecision.KKYear: return "KKYear";
                    case TLDatePrecision.KKKYear: return "KKKYear";
                    case TLDatePrecision.MYear: return "MYear";
                    case TLDatePrecision.KMYear: return "KMYear";
                    case TLDatePrecision.KKMYear: return "KKMYear";
                    case TLDatePrecision.BYear: return "BYear";
                    default: return "";
                }
            }
        }
        public string DateStrFull
        {
            get
            {
                string ret = YearStr;
                if (bcacDate != null)
                {
                    ret = ret + "." + bcacDate.Value.Month.ToString() + "." + bcacDate.Value.Day.ToString() + ". ";
                    ret = ret + bcacDate.Value.Hour.ToString() + ":" + bcacDate.Value.Minute.ToString();
                }
                return ret;
            }
        }

        public TLDate(string yearstr)
        {
            if (yearstr.ToLower().EndsWith("b"))        //billion years
            {
                yearstr = yearstr.ToLower().Replace("b", "");
                precision = TLDatePrecision.BYear;
                exactYear = int.Parse(yearstr) * MultiplierForPrecision(precision);
            }
            else if (yearstr.ToLower().EndsWith("m"))   //million years
            {
                yearstr = yearstr.ToLower().Replace("m", "");
                precision = TLDatePrecision.MYear;

                while (yearstr.EndsWith("0") && precision < PRECISION_MAX)
                {
                    precision += 1;
                    yearstr = yearstr.Substring(0, yearstr.Length - 1);
                    if (String.IsNullOrEmpty(yearstr)) throw new ArgumentException("Incorrect value");
                }

                exactYear = int.Parse(yearstr) * MultiplierForPrecision(precision);
            }
            else if (yearstr.ToLower().EndsWith("k"))   //thousand years
            {
                yearstr = yearstr.ToLower().Replace("k", "");
                precision = TLDatePrecision.KYear;

                while (yearstr.EndsWith("0") && precision < PRECISION_MAX)
                {
                    precision += 1;
                    yearstr = yearstr.Substring(0, yearstr.Length - 1);
                    if (String.IsNullOrEmpty(yearstr)) throw new ArgumentException("Incorrect value");
                }

                exactYear = long.Parse(yearstr) * MultiplierForPrecision(precision);
            }

            if (Math.Abs(exactYear) > MAX_YEAR) throw new OverflowException("Maximum 99 billion years");

            bcacDate = null;
            if (Math.Abs(exactYear) < 10000)
            {
                bcac = exactYear < 0 ? BCAC.BC : BCAC.AC;
                Initialize((int)exactYear);
            }
            BCACDateChanged();
        }

        public TLDate(Int64 year, int month = -1, int day = -1, int hour = -1, int minute = -1)
        {
            bcac = year < 0 ? BCAC.BC : BCAC.AC;
            if (Math.Abs(year) < 10000)
            {
                exactYear = year;
                Initialize((int)year, month, day, hour, minute);
            }
            else
            {
                bcacDate = null;
                precision = TLDatePrecision.Year;
                exactYear = year; // year < 0 ? -year : year;
            }
            BCACDateChanged();
        }

        private void Initialize(int year, int month = -1, int day = -1, int hour = -1, int minute = -1)
        {
            if (year < 0) year = year + 10000;

            if (month == -1)
            {
                bcacDate = new DateTime(year, 1, 1);
                precision = TLDatePrecision.Year;
            }
            else if (day == -1)
            {
                bcacDate = new DateTime(year, month, 1);
                precision = TLDatePrecision.Month;
            }
            else if (hour == -1)
            {
                bcacDate = new DateTime(year, month, day);
                precision = TLDatePrecision.Day;
            }
            else if (minute == -1)
            {
                bcacDate = new DateTime(year, month, day, hour, 0, 0);
                precision = TLDatePrecision.Hour;
            }
            else
            {
                bcacDate = new DateTime(year, month, day, hour, minute, 0);
                precision = TLDatePrecision.Minute;
            }
        }

        private long MultiplierForPrecision(TLDatePrecision precision)
        {
            switch (precision)
            {
                case TLDatePrecision.KYear: return 1000;
                case TLDatePrecision.KKYear: return 10000;
                case TLDatePrecision.KKKYear: return 100000;
                case TLDatePrecision.MYear: return 1000000;
                case TLDatePrecision.KMYear: return 10000000;
                case TLDatePrecision.KKMYear: return 100000000;
                case TLDatePrecision.BYear: return 1000000000;
                default: return 0;
            }
        }

        public override void AddMinutes(int count)
        {
            try { base.AddMinutes(count); }
            catch (OverflowException)
            {
                exactYear = count > 0 ? 10000 : -10000;
                bcacDate = null;
                BCACDateChanged();
            }
        }

        public override void AddHours(int count)
        {
            try { base.AddHours(count); }
            catch (OverflowException)
            {
                exactYear = count > 0 ? 10000 : -10000;
                bcacDate = null;
                BCACDateChanged();
            }
        }

        public override void AddDays(int count)
        {
            try { base.AddDays(count); }
            catch (OverflowException)
            {
                exactYear = count > 0 ? 10000 : -10000;
                bcacDate = null;
                BCACDateChanged();
            }
        }

        public override void AddMonths(int count)
        {
            try { base.AddMonths(count); }
            catch (OverflowException)
            {
                exactYear = Year + (count / 12);
                if (count > 0)
                    if (bcacDate.Value.Month + (count % 12) > 12) exactYear++;
                if (count < 0)
                    if (bcacDate.Value.Month + (count % 12) < 1) exactYear--;

                bcacDate = null;
                BCACDateChanged();
            }
        }

        public override void AddYears(int count)
        {
            try { base.AddYears(count); }
            catch (OverflowException)
            {
                exactYear = exactYear + count;
                bcacDate = null;
                BCACDateChanged();
            }
        }

        protected override void BCACDateChanged()
        {
            if (bcacDate == null)
            {
                TotalMinutes = (Int64)(exactYear * MINUTES_PER_YEAR);
            }
            else
            {
                if (bcac == BCAC.BC)
                    TotalMinutes = (bcacDate.Value.Ticks - (DateTime.MaxValue.Ticks + 1)) / TICKS_PER_MINUTE;
                else
                    TotalMinutes = bcacDate.Value.Ticks / TICKS_PER_MINUTE;
            }
        }
    }
}
