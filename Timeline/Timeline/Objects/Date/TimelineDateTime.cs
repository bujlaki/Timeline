using System;

using Timeline.Controls;

namespace Timeline.Objects.Date
{
    public class TimelineDateTime : BCACDateTime
    {
        public int Decade { get; set; }
        public int Century { get; set; }
		public TimelineUnits Precision { get; set; }

        public static new TimelineDateTime MaxValue
        {
            get
            {
                var ret = TimelineDateTime.FromTicks(DateTime.MaxValue.Ticks);
                ret.Decade = ret.Year / 10;
                ret.Century = ret.Year / 100;
                return ret;
            }
        }

        public static new TimelineDateTime MinValue
        {
            get
            {
                var ret = TimelineDateTime.FromTicks(-DateTime.MaxValue.Ticks);
                ret.Decade = ret.Year / 10;
                ret.Century = ret.Year / 100;
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
            return tldate;
        }

        public static TimelineDateTime FromTicksCapped(Int64 ticks)
        {
            if (ticks > MaxTicks) ticks = MaxTicks;
            if (ticks < MinTicks) ticks = MinTicks;
            return TimelineDateTime.FromTicks(ticks);
        }

        public TimelineDateTime() : base() { }

		public TimelineDateTime(DateTime dateTime, BCAC bc_or_ac = BCAC.AC) : base(dateTime, bc_or_ac)
		{
			Precision = TimelineUnits.Minute;
			Decade = Year / 10;
            Century = Year / 100;
		}

        public TimelineDateTime(int year, int month=-1, int day=-1, int hour=-1, int minute=-1)
        {
			if(month==-1)
			{
                SetDate(year, 1, 1, 0, 0);
				Precision = TimelineUnits.Year;
			}
			else if(day == -1)
			{
                SetDate(year, month, 1, 0, 0);
				Precision = TimelineUnits.Month;
			}
			else if(hour==-1)
			{
				SetDate(year, month, day, 0, 0);
				Precision = TimelineUnits.Day;
			}
			else if(minute==-1)
			{
				SetDate(year, month, day, hour, 0);
				Precision = TimelineUnits.Hour;
			}
			else
			{
				SetDate(year, month, day, hour, minute);
                Precision = TimelineUnits.Minute;
			}
			Decade = Year / 10;
			Century = Year / 100;
        }

        public string DateStr(TimelineUnits unit)
        {
            switch (unit)
            {
                case TimelineUnits.Minute:
                    return Year.ToString() + "." + Month.ToString() + "." + Day.ToString() + " " + Hour.ToString() + ":" + Minute.ToString();
                case TimelineUnits.Hour:
                    return Year.ToString() + "." + Month.ToString() + "." + Day.ToString() + " " + Hour.ToString() + ":00";
                case TimelineUnits.Day:
					return Year.ToString() + "." + Month.ToString() + "." + Day.ToString();
                case TimelineUnits.Month:
					return Year.ToString() + "." + Month.ToString();
                case TimelineUnits.Year:
					return Year.ToString();
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
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.SetDate(Year, Month, Day, Hour, Minute);
					break;
				case TimelineUnits.Hour:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.SetDate(Year, Month, Day, Hour, 0);
					break;
				case TimelineUnits.Day:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.SetDate(Year, Month, Day, 0, 0);
					break;
				case TimelineUnits.Month:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.SetDate(Year, Month, 1, 0, 0);
					break;
				case TimelineUnits.Year:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.SetDate(Year, 1, 1, 0, 0);
					break;
				case TimelineUnits.Decade:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.SetDate(Decade * 10, 1, 1, 0, 0);
					break;
				case TimelineUnits.Century:
					dstDate.Century = Century;
					dstDate.Decade = Century * 10;
					dstDate.SetDate(Century * 100, 1, 1, 0, 0);
					break;
			}

			dstDate.Precision = precision;
		}

        public void Add(int value=1)
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
            Decade = Year / 10;
            Century = Year / 100;
        }

        public void AddCapped(int value = 1)
        {
            AddCapped(this.Precision, value);
        }

        public void AddCapped(TimelineUnits unit, int value = 1)
        {
            try
            {
                switch (unit)
                {
                    case TimelineUnits.Minute: AddMinutes(value); break;
                    case TimelineUnits.Hour:   AddHours(value); break;
                    case TimelineUnits.Day:    AddDays(value); break;
                    case TimelineUnits.Month:  AddMonths(value); break;
                    case TimelineUnits.Year:   AddYears(value); break;
                    case TimelineUnits.Decade: AddYears(10 * value); break;
                    case TimelineUnits.Century:AddYears(100 * value); break;
                }
                Decade = Year / 10;
                Century = Year / 100;
            }
            catch (OverflowException)
            {
                Ticks = value > 0 ? MaxTicks : MinTicks;
            }

        }
    }
}
