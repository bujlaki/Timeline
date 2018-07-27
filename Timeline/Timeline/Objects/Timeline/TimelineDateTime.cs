using System;

using Timeline.Controls;

namespace Timeline.Objects.Timeline
{
    public class TimelineDateTime : BCACDateTime
    {
        public int Decade
        {
            get
            {
                return Year / 10;
                //int d = Year / 10;
                //if (Year < 0) return d - 1;
                //return d + 1;
            }
        }
        public int Century
        {
            get
            {
                return Year / 100;
                //int c = Year / 100;
                //if (Year < 0) return c - 1;
                //return c + 1;
            }
        }
        public string DecadeStr
        {
            get
            {
                int d = Decade;
                if (d < 0) return "BC" + (-d).ToString() + "0";
                return d.ToString() + "0";
            }
        }
        public string CenturyStr
        {
            get
            {
                int c = Century;
                if (c < 0) return "BC" + (-c).ToString() + "00";
                return c.ToString() + "00";
            }
        }
        public TimelineUnits Precision { get; set; }

        public static new TimelineDateTime MaxValue
        {
            get
            {
                var ret = TimelineDateTime.FromTicks(DateTime.MaxValue.Ticks);
                return ret;
            }
        }

        public static new TimelineDateTime MinValue
        {
            get
            {
                var ret = TimelineDateTime.FromTicks(-DateTime.MaxValue.Ticks);
                return ret;
            }
        }

        public static TimelineDateTime FromTicks(Int64 ticks)
        {
            TimelineDateTime tldate = new TimelineDateTime();
            if (ticks > 0)
            {
                tldate.bcac = BCAC.AC;
                tldate.bcacDate = new DateTime(ticks);
            }
            else
            {
                tldate.bcac = BCAC.BC;
                tldate.bcacDate = new DateTime(ticks + DateTime.MaxValue.Ticks);
            }
            tldate.Precision = TimelineUnits.Minute;
            return tldate;
        }

        public static TimelineDateTime FromTicksCapped(Int64 ticks)
        {
            if (ticks > TimelineDateTime.MaxTicks) return TimelineDateTime.MaxValue;
            if (ticks < TimelineDateTime.MinTicks) return TimelineDateTime.MinValue;
            return TimelineDateTime.FromTicks(ticks);
        }

        public TimelineDateTime() : base() { }

        public TimelineDateTime(DateTime dateTime, BCAC bc_or_ac = BCAC.AC) : base(dateTime, bc_or_ac)
        {
            Precision = TimelineUnits.Minute;
        }

        public TimelineDateTime(int year, int month = -1, int day = -1, int hour = -1, int minute = -1)
        {
            if (month == -1)
            {
                SetDate(year, 1, 1, 0, 0);
                Precision = TimelineUnits.Year;
            }
            else if (day == -1)
            {
                SetDate(year, month, 1, 0, 0);
                Precision = TimelineUnits.Month;
            }
            else if (hour == -1)
            {
                SetDate(year, month, day, 0, 0);
                Precision = TimelineUnits.Day;
            }
            else if (minute == -1)
            {
                SetDate(year, month, day, hour, 0);
                Precision = TimelineUnits.Hour;
            }
            else
            {
                SetDate(year, month, day, hour, minute);
                Precision = TimelineUnits.Minute;
            }
        }

        public string DateStr()
        {
            return DateStr(Precision);
        }

        public string DateStr(TimelineUnits unit)
        {
            switch (unit)
            {
                case TimelineUnits.Minute:
                    return YearStr + "." + Month.ToString() + "." + Day.ToString() + " " + Hour.ToString() + ":" + Minute.ToString();
                case TimelineUnits.Hour:
                    return YearStr + "." + Month.ToString() + "." + Day.ToString() + " " + Hour.ToString() + ":00";
                case TimelineUnits.Day:
                    return YearStr + "." + Month.ToString() + "." + Day.ToString();
                case TimelineUnits.Month:
                    return YearStr + "." + Month.ToString();
                case TimelineUnits.Year:
                    return YearStr;
                case TimelineUnits.Decade:
                    return Decade.ToString();
                default:
                    return "";
            }
        }

        public void CopyTo(ref TimelineDateTime dstDate)
        {
            CopyTo(ref dstDate, this.Precision);
        }

        public void CopyTo(ref TimelineDateTime dstDate, TimelineUnits precision)
        {
            switch (precision)
            {
                case TimelineUnits.Minute:
                    dstDate.SetDate(Year, Month, Day, Hour, Minute);
                    break;
                case TimelineUnits.Hour:
                    dstDate.SetDate(Year, Month, Day, Hour, 0);
                    break;
                case TimelineUnits.Day:
                    dstDate.SetDate(Year, Month, Day, 0, 0);
                    break;
                case TimelineUnits.Month:
                    dstDate.SetDate(Year, Month, 1, 0, 0);
                    break;
                case TimelineUnits.Year:
                    dstDate.SetDate(Year, 1, 1, 0, 0);
                    break;
                case TimelineUnits.Decade:
                    if (Decade == 0)
                        dstDate.SetDate(1, 1, 1, 0, 0);
                    else
                        dstDate.SetDate(Decade * 10, 1, 1, 0, 0);
                    break;
                case TimelineUnits.Century:
                    dstDate.SetDate(Century * 100, 1, 1, 0, 0);
                    break;
            }

            dstDate.Precision = precision;
        }

        public void Add(int value = 1)
        {
            Add(this.Precision, value);
        }

        public void Add(TimelineUnits unit, int value = 1)
        {
            switch (unit)
            {
                case TimelineUnits.Minute: AddMinutes(value); break;
                case TimelineUnits.Hour: AddHours(value); break;
                case TimelineUnits.Day: AddDays(value); break;
                case TimelineUnits.Month: AddMonths(value); break;
                case TimelineUnits.Year: AddYears(value); break;
                case TimelineUnits.Decade: AddYears(10 * value); break;
                case TimelineUnits.Century: AddYears(100 * value); break;
            }
        }

    }
}
