using System;

namespace Timeline.Objects.Date
{
    public class BCACDateTime
    {
        public enum BCAC
        {
            BC,
            AC
        }

        const Int64 TICKS_PER_MINUTE = 600000000;
        const Int64 TICKS_PER_HOUR  = 36000000000;
        const Int64 TICKS_PER_DAY   = 864000000000;
        const int MAX_YEARS = 9999;

        protected DateTime? bcacDate { get; set; }
        protected BCAC bcac { get; set; }

        public BCACDateTime() : this(null) { }
        public BCACDateTime(DateTime? dateTime, BCAC bc_or_ac = BCAC.AC)
        {
            bcacDate = dateTime;
            bcac = bc_or_ac;
        }

        protected class DateChangedArgs
        {
            public bool AtMax { get; set; }
            public bool AtMin { get; set; }
            public bool BCACChanged { get; set; }
            public BCAC BcAc { get; set; }
            public DateChangedArgs(bool isAtMax, bool isAtMin, bool bcacChanged, BCAC bcac)
            {
                AtMax = isAtMax;
                AtMin = isAtMin;
                BCACChanged = bcacChanged;
                BcAc = bcac;
            }
        }

        public void AddMinutes(int count)
        {
            try
            {
                bcacDate = bcacDate?.AddMinutes(count);
                BCACDateChanged(new DateChangedArgs(false, false, false, bcac));
            }
            catch (ArgumentOutOfRangeException)
            {
                if (bcac==BCAC.AC && count>0)
                {
                    bcacDate = DateTime.MaxValue;
                    BCACDateChanged(new DateChangedArgs(true, false, false, bcac));
                }
                else if (bcac==BCAC.BC && count < 0)
                {
                    bcacDate = DateTime.MinValue;
                    BCACDateChanged(new DateChangedArgs(false, true, false, bcac));
                }
                else
                {
                    Int64 ticks = bcacDate.Value.Ticks + count * TICKS_PER_MINUTE;
                    while (ticks < 0 || ticks > DateTime.MaxValue.Ticks)
                    {
                        if (ticks < 0) ticks += (DateTime.MaxValue.Ticks + 1);
                        if (ticks > DateTime.MaxValue.Ticks) ticks -= (DateTime.MaxValue.Ticks + 1);
                    }
                    bcacDate = new DateTime(ticks);
                    BCACDateChanged(new DateChangedArgs(false, false, true, bcac));
                }

            }
        }

        public void AddHours(int count)
        {
            try
            {
                bcacDate = bcacDate?.AddHours(count);
                BCACDateChanged(new DateChangedArgs(false, false, false, bcac));
            }
            catch (ArgumentOutOfRangeException)
            {
                if (bcac == BCAC.AC && count > 0)
                {
                    bcacDate = DateTime.MaxValue;
                    BCACDateChanged(new DateChangedArgs(true, false, false, bcac));
                }
                else if (bcac == BCAC.BC && count < 0)
                {
                    bcacDate = DateTime.MinValue;
                    BCACDateChanged(new DateChangedArgs(false, true, false, bcac));
                }
                else
                {
                    Int64 ticks = bcacDate.Value.Ticks + count * TICKS_PER_HOUR;
                    while (ticks < 0 || ticks > DateTime.MaxValue.Ticks)
                    {
                        if (ticks < 0)
                        {
                            ticks += (DateTime.MaxValue.Ticks + 1);
                        }
                        if (ticks > DateTime.MaxValue.Ticks)
                        {
                            ticks -= (DateTime.MaxValue.Ticks + 1);
                        }
                    }
                    bcacDate = new DateTime(ticks);
                    BCACDateChanged(new DateChangedArgs(false, false, true, bcac));
                }
            }
        }

        public void AddDays(int count)
        {
            try
            {
                bcacDate = bcacDate?.AddDays(count);
                BCACDateChanged(new DateChangedArgs(false, false, false, bcac));
            }
            catch (ArgumentOutOfRangeException)
            {
                if (bcac == BCAC.AC && count > 0)
                {
                    bcacDate = DateTime.MaxValue;
                    BCACDateChanged(new DateChangedArgs(true, false, false, bcac));
                }
                else if (bcac == BCAC.BC && count < 0)
                {
                    bcacDate = DateTime.MinValue;
                    BCACDateChanged(new DateChangedArgs(false, true, false, bcac));
                }
                else
                {
                    Int64 ticks = bcacDate.Value.Ticks + count * TICKS_PER_DAY;
                    while (ticks < 0 || ticks > DateTime.MaxValue.Ticks)
                    {
                        if (ticks < 0)
                        {
                            ticks += (DateTime.MaxValue.Ticks + 1);
                        }
                        if (ticks > DateTime.MaxValue.Ticks)
                        {
                            ticks -= (DateTime.MaxValue.Ticks + 1);
                        }
                    }
                    bcacDate = new DateTime(ticks);
                    BCACDateChanged(new DateChangedArgs(false, false, true, bcac));
                }
            }
        }

        public void AddMonths(int count)
        {
            int totalToAdd = count;
            try
            {
                if (Math.Abs(totalToAdd) > 120000)
                {
                    totalToAdd = totalToAdd % 120000;
                }
                if (totalToAdd < 0)
                {
                    totalToAdd += MAX_YEARS * 12;
                }
                bcacDate = bcacDate?.AddMonths(totalToAdd);
                BCACDateChanged(new DateChangedArgs(false, false, false, bcac));
            }
            catch (ArgumentOutOfRangeException)
            {
                if (bcac == BCAC.AC && count > 0)
                {
                    bcacDate = DateTime.MaxValue;
                    BCACDateChanged(new DateChangedArgs(true, false, false, bcac));
                }
                else if (bcac == BCAC.BC && count < 0)
                {
                    bcacDate = DateTime.MinValue;
                    BCACDateChanged(new DateChangedArgs(false, true, false, bcac));
                }
                else
                {
                    bcacDate = bcacDate?.AddMonths(totalToAdd - MAX_YEARS * 12);
                    BCACDateChanged(new DateChangedArgs(false, false, true, bcac));
                }
            }
        }

        public void AddYears(int count)
        {
            int totalToAdd = count;
            try
            {
                if (Math.Abs(totalToAdd) > 10000)
                {
                    totalToAdd = totalToAdd % 10000;
                }
                if (totalToAdd < 0)
                {
                    totalToAdd += MAX_YEARS;
                }
                bcacDate = bcacDate?.AddYears(totalToAdd);
                BCACDateChanged(new DateChangedArgs(false, false, false, bcac));
            }
            catch (ArgumentOutOfRangeException)
            {
                if (bcac == BCAC.AC && count > 0)
                {
                    bcacDate = DateTime.MaxValue;
                    BCACDateChanged(new DateChangedArgs(true, false, false, bcac));
                }
                else if (bcac == BCAC.BC && count < 0)
                {
                    bcacDate = DateTime.MinValue;
                    BCACDateChanged(new DateChangedArgs(false, true, false, bcac));
                }
                else
                {
                    bcacDate = bcacDate?.AddYears(totalToAdd - MAX_YEARS);
                    BCACDateChanged(new DateChangedArgs(false, false, true, bcac));
                }
            }
        }

        protected virtual void BCACDateChanged(DateChangedArgs args) { }

    }
}
