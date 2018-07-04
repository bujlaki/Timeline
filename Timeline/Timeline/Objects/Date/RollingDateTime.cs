using System;

namespace Timeline.Objects.Date
{
    public class RollingDateTime
    {
        const Int64 TICKS_PER_MINUTE = 600000000;
        const Int64 TICKS_PER_HOUR  = 36000000000;
        const Int64 TICKS_PER_DAY   = 864000000000;
        const int MAX_YEARS = 9999;

        public DateTime Value { get; set; }

        public RollingDateTime() : this(DateTime.UtcNow) { }
        public RollingDateTime(DateTime dateTime) { Value = dateTime; }

        protected class RollEventArgs
        {
            public int Count { get; set; }
            public RollEventArgs(int count) { Count = count; }
        }

        protected delegate void RollEventHandler(RollEventArgs e);

        protected event RollEventHandler OnRollOver;

        public void AddMinutes(int count)
        {
            try
            {
                Value = Value.AddMinutes(count);
                DateChanged();
            }
            catch (ArgumentOutOfRangeException)
            {
                Int64 ticks = Value.Ticks + count * TICKS_PER_MINUTE;
                while (ticks < 0 || ticks > DateTime.MaxValue.Ticks)
                {
                    if (ticks < 0) {
                        ticks += (DateTime.MaxValue.Ticks + 1);
                        RollOver(-1);
                    }
                    if (ticks > DateTime.MaxValue.Ticks) {
                        ticks -= (DateTime.MaxValue.Ticks + 1);
                        RollOver(1);
                    }
                }
                Value = new DateTime(ticks);
                DateChanged();
            }
        }

        public void AddHours(int count)
        {
            try
            {
                Value = Value.AddHours(count);
                DateChanged();
            }
            catch (ArgumentOutOfRangeException)
            {
                Int64 ticks = Value.Ticks + count * TICKS_PER_HOUR;
                while (ticks < 0 || ticks > DateTime.MaxValue.Ticks)
                {
                    if (ticks < 0) {
                        ticks += (DateTime.MaxValue.Ticks + 1);
                        RollOver(-1);
                    }
                    if (ticks > DateTime.MaxValue.Ticks) {
                        ticks -= (DateTime.MaxValue.Ticks + 1);
                        RollOver(1);
                    }
                }
                Value = new DateTime(ticks);
                DateChanged();
            }
        }

        public void AddDays(int count)
        {
            try
            {
                Value = Value.AddDays(count);
                DateChanged();
            }
            catch (ArgumentOutOfRangeException)
            {
                Int64 ticks = Value.Ticks + count * TICKS_PER_DAY;
                while (ticks < 0 || ticks > DateTime.MaxValue.Ticks)
                {
                    if (ticks < 0) {
                        ticks += (DateTime.MaxValue.Ticks + 1);
                        RollOver(-1);
                    }
                    if (ticks > DateTime.MaxValue.Ticks) {
                        ticks -= (DateTime.MaxValue.Ticks + 1);
                        RollOver(1);
                    }
                }
                Value = new DateTime(ticks);
                DateChanged();
            }
        }

        public void AddMonths(int count)
        {
            int totalToAdd = count;
            try
            {
                if (Math.Abs(totalToAdd) > 120000)
                {
                    RollOver(totalToAdd / 120000);
                    totalToAdd = totalToAdd % 120000;
                }
                if (totalToAdd < 0)
                {
                    totalToAdd += MAX_YEARS * 12;
                    RollOver(-1);
                }
                Value = Value.AddMonths(totalToAdd);
                DateChanged();
            }
            catch (ArgumentOutOfRangeException)
            {
                Value = Value.AddMonths(totalToAdd - MAX_YEARS * 12);
                RollOver(1);
                DateChanged();
            }
        }

        public void AddYears(int count)
        {
            int totalToAdd = count;
            try
            {
                if (Math.Abs(totalToAdd) > 10000)
                {
                    RollOver(totalToAdd / 10000);
                    totalToAdd = totalToAdd % 10000;
                }
                if (totalToAdd < 0)
                {
                    totalToAdd += MAX_YEARS;
                    RollOver(-1);
                }
                Value = Value.AddYears(totalToAdd);
                DateChanged();
            }
            catch (ArgumentOutOfRangeException)
            {
                Value = Value.AddYears(totalToAdd - MAX_YEARS);
                RollOver(1);
                DateChanged();
            }
        }

        protected virtual void RollOver(int count) { }
        protected virtual void DateChanged() { }
    }
}
