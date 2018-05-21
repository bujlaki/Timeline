using System;

using Timeline.Controls;

namespace Timeline.Models
{
    public class MTimelineDate
    {
        public DateTime BaseDate { get; set; }
        public int Decade { get; set; }
        public int Century { get; set; }
		public TimelineUnits Precision { get; set; }

		public MTimelineDate(DateTime _date)
		{
			BaseDate = _date;
			Precision = TimelineUnits.Minute;
			Decade = BaseDate.Year / 10;
            Century = BaseDate.Year / 100;
		}

        public MTimelineDate(int year, int month=-1, int day=-1, int hour=-1, int minute=-1)
        {
			if(month==-1)
			{
				BaseDate = new DateTime(2018, 1, 1);
				Precision = TimelineUnits.Year;
			}
			else if(day == -1)
			{
				BaseDate = new DateTime(year, month, 1);
				Precision = TimelineUnits.Month;
			}
			else if(hour==-1)
			{
				BaseDate = new DateTime(year, month, day);
				Precision = TimelineUnits.Day;
			}
			else if(minute==-1)
			{
				BaseDate = new DateTime(year, month, day, hour, 0, 0);
				Precision = TimelineUnits.Hour;
			}
			else
			{
				BaseDate = new DateTime(year, month, day, hour, minute, 0);
                Precision = TimelineUnits.Minute;
			}
			Decade = BaseDate.Year / 10;
			Century = BaseDate.Year / 100;
        }

        public string DateStr(TimelineUnits unit)
        {
            switch (unit)
            {
                case TimelineUnits.Minute:
					return BaseDate.ToShortDateString() + "  " + BaseDate.ToShortTimeString();
                case TimelineUnits.Hour:
					return BaseDate.ToShortDateString() + "  " + BaseDate.ToString("HH:00");
                case TimelineUnits.Day:
					return BaseDate.ToShortDateString();
                case TimelineUnits.Month:
					return BaseDate.ToString("yyyy.MM");
                case TimelineUnits.Year:
					return BaseDate.Year.ToString();
                case TimelineUnits.Decade:
                    return Decade.ToString();
                default:
                    return "";
            }
        }
        
        public void CopyTo(ref MTimelineDate dstDate)
        {
			CopyTo(ref dstDate, this.Precision);
			//dstDate.BaseDate = BaseDate;
            //dstDate.Decade = Decade;
            //dstDate.Century = Century;
			//dstDate.Precision = Precision;
        }

		public void CopyTo(ref MTimelineDate dstDate, TimelineUnits precision)
		{
			switch (precision)
			{
				case TimelineUnits.Minute:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.BaseDate = new DateTime(BaseDate.Year, BaseDate.Month, BaseDate.Day, BaseDate.Hour, BaseDate.Minute, 0);
					break;
				case TimelineUnits.Hour:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.BaseDate = new DateTime(BaseDate.Year, BaseDate.Month, BaseDate.Day, BaseDate.Hour, 0, 0);
					break;
				case TimelineUnits.Day:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.BaseDate = new DateTime(BaseDate.Year, BaseDate.Month, BaseDate.Day, 0, 0, 0);
					break;
				case TimelineUnits.Month:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.BaseDate = new DateTime(BaseDate.Year, BaseDate.Month, 1, 0, 0, 0);
					break;
				case TimelineUnits.Year:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.BaseDate = new DateTime(BaseDate.Year, 1, 1, 0, 0, 0);
					break;
				case TimelineUnits.Decade:
					dstDate.Century = Century;
					dstDate.Decade = Decade;
					dstDate.BaseDate = new DateTime(Decade * 10, 1, 1, 0, 0, 0);
					break;
				case TimelineUnits.Century:
					dstDate.Century = Century;
					dstDate.Decade = Century * 10;
					dstDate.BaseDate = new DateTime(Century * 100, 1, 1, 0, 0, 0);
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
            switch(unit)
            {
                case TimelineUnits.Minute:
					BaseDate = BaseDate.AddMinutes(value);
                    break;
                case TimelineUnits.Hour:
					BaseDate = BaseDate.AddHours(value);
                    break;
                case TimelineUnits.Day:
					BaseDate = BaseDate.AddDays(value);
                    break;
                case TimelineUnits.Month:
					BaseDate = BaseDate.AddMonths(value);
					Decade = BaseDate.Year / 10;
					Century = BaseDate.Year / 100;
                    break;
                case TimelineUnits.Year:
					BaseDate = BaseDate.AddYears(value);
					Decade = BaseDate.Year / 10;
					Century = BaseDate.Year / 100;
                    break;
                case TimelineUnits.Decade:
					BaseDate = BaseDate.AddYears(10 * value);
					Decade = BaseDate.Year / 10;
					Century = BaseDate.Year / 100;
                    break;
                case TimelineUnits.Century:
					BaseDate = BaseDate.AddYears(100 * value);
					Decade = BaseDate.Year / 10;
					Century = BaseDate.Year / 100;
                    break;
            }
        }
    }
}
