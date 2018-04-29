using System;
namespace Timeline.Controls
{
    public class TimelineControlDate
    {
        public DateTime baseDate { get; set; }
        public int Decade { get; set; }
        public int Century { get; set; }
        public int KKYear { get; set; }
        public int KKKYear { get; set; }

        public TimelineControlDate()
        {
            baseDate = new DateTime(2018, 1, 1);
            Decade = baseDate.Year / 10;
            Century = baseDate.Year / 100;
            KKYear = 0;
            KKKYear = 0;
        }

        public string DateStr()
        {
            return baseDate.ToShortDateString() + "  " + baseDate.ToShortTimeString();    
        }

        public void Copy(ref TimelineControlDate dstDate)
        {
            dstDate.baseDate = baseDate;
            dstDate.Decade = Decade;
            dstDate.Century = Century;
            dstDate.KKYear = KKYear;
            dstDate.KKKYear = KKKYear;
        }

        public void CopyByUnit(ref TimelineControlDate dstDate, TimelineUnits unit)
        {
            switch (unit)
            {
                case TimelineUnits.Minute:
                    dstDate.Century = Century;
                    dstDate.Decade = Decade;
                    dstDate.baseDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, baseDate.Hour, baseDate.Minute, 0);
                    break;
                case TimelineUnits.Hour:
                    dstDate.Century = Century;
                    dstDate.Decade = Decade;
                    dstDate.baseDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, baseDate.Hour, 0, 0);
                    break;
                case TimelineUnits.Day:
                    dstDate.Century = Century;
                    dstDate.Decade = Decade;
                    dstDate.baseDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);
                    break;
                case TimelineUnits.Month:
                    dstDate.Century = Century;
                    dstDate.Decade = Decade;
                    dstDate.baseDate = new DateTime(baseDate.Year, baseDate.Month, 1, 0, 0, 0);
                    break;
                case TimelineUnits.Year:
                    dstDate.Century = Century;
                    dstDate.Decade = Decade;
                    dstDate.baseDate = new DateTime(baseDate.Year, 1, 1, 0, 0, 0);
                    break;
                case TimelineUnits.Decade:
                    dstDate.Century = Century;
                    dstDate.Decade = Decade;
                    dstDate.baseDate = new DateTime(Decade * 10, 1, 1, 0, 0, 0);
                    break;
                case TimelineUnits.Century:
                    dstDate.Century = Century;
                    dstDate.Decade = Century * 10;
                    dstDate.baseDate = new DateTime(Century * 100, 1, 1, 0, 0, 0);
                    break;
            }

        }

        public int Value(TimelineUnits unit)
        {
            switch (unit)
            {
                case TimelineUnits.Minute:
                    return baseDate.Minute;
                case TimelineUnits.Hour:
                    return baseDate.Hour;
                case TimelineUnits.Day:
                    return baseDate.Day;
                case TimelineUnits.Month:
                    return baseDate.Month;
                case TimelineUnits.Year:
                    return baseDate.Year;
                case TimelineUnits.Decade:
                    return Decade;
                case TimelineUnits.Century:
                    return Century;
                case TimelineUnits.KKYear:
                    return KKYear;
                case TimelineUnits.KKKYear:
                    return KKKYear;
                default:
                    return 0;
            }
        }

        public int YearsInDecade()
        {
            return baseDate.Year % 10;
        }

        public int DecadesInCentury()
        {
            return Decade % 10;
        }

        public void Add(TimelineUnits unit, int value = 1)
        {
            switch(unit)
            {
                case TimelineUnits.Minute:
                    baseDate = baseDate.AddMinutes(value);
                    break;
                case TimelineUnits.Hour:
                    baseDate = baseDate.AddHours(value);
                    break;
                case TimelineUnits.Day:
                    baseDate = baseDate.AddDays(value);
                    break;
                case TimelineUnits.Month:
                    baseDate = baseDate.AddMonths(value);
                    Decade = baseDate.Year / 10;
                    Century = baseDate.Year / 100;
                    break;
                case TimelineUnits.Year:
                    baseDate = baseDate.AddYears(value);
                    Decade = baseDate.Year / 10;
                    Century = baseDate.Year / 100;
                    break;
                case TimelineUnits.Decade:
                    baseDate = baseDate.AddYears(10 * value);
                    Decade = baseDate.Year / 10;
                    Century = baseDate.Year / 100;
                    break;
                case TimelineUnits.Century:
                    baseDate = baseDate.AddYears(100 * value);
                    Decade = baseDate.Year / 10;
                    Century = baseDate.Year / 100;
                    break;
                case TimelineUnits.KKYear:
                    KKYear += value;
                    break;
                case TimelineUnits.KKKYear:
                    KKKYear += value;
                    break;
            }
        }
    }
}
