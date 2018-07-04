using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Objects.Date
{
    class RollingDateTime
    {
        const Int64 tickspermin = 600000000;
        const Int64 ticksperhour = 36000000000;
        const Int64 ticksperday = 864000000000;
        const int maxyear = 9999;

        private static void AddMinutes(ref DateTime dt, int count)
        {
            try
            {
                dt = dt.AddMinutes(count);
            }
            catch (ArgumentOutOfRangeException)
            {
                Int64 ticks = dt.Ticks + count * tickspermin;
                while (ticks < 0 || ticks > DateTime.MaxValue.Ticks)
                {
                    if (ticks < 0) { ticks += (DateTime.MaxValue.Ticks + 1); }
                    if (ticks > DateTime.MaxValue.Ticks) { ticks -= (DateTime.MaxValue.Ticks + 1); }
                }
                dt = new DateTime(ticks);
            }
        }

        private static void AddHours(ref DateTime dt, int count)
        {
            try
            {
                dt = dt.AddHours(count);
            }
            catch (ArgumentOutOfRangeException)
            {
                Int64 ticks = dt.Ticks + count * ticksperhour;
                while (ticks < 0 || ticks > DateTime.MaxValue.Ticks)
                {
                    if (ticks < 0) { ticks += (DateTime.MaxValue.Ticks + 1); }
                    if (ticks > DateTime.MaxValue.Ticks) { ticks -= (DateTime.MaxValue.Ticks + 1); }
                }
                dt = new DateTime(ticks);
            }
        }

        private static void AddDays(ref DateTime dt, int count)
        {
            try
            {
                dt = dt.AddDays(count);
            }
            catch (ArgumentOutOfRangeException)
            {
                Int64 ticks = dt.Ticks + count * ticksperday;
                while (ticks < 0 || ticks > DateTime.MaxValue.Ticks)
                {
                    if (ticks < 0) { ticks += (DateTime.MaxValue.Ticks + 1); }
                    if (ticks > DateTime.MaxValue.Ticks) { ticks -= (DateTime.MaxValue.Ticks + 1); }
                }
                dt = new DateTime(ticks);
            }
        }

        private static void AddMonths(ref DateTime dt, int count)
        {
            int totalToAdd = count;
            try
            {
                if (Math.Abs(totalToAdd) > 120000) totalToAdd = totalToAdd % 120000;
                if (totalToAdd < 0) totalToAdd += maxyear * 12;
                dt = dt.AddMonths(totalToAdd);
            }
            catch (ArgumentOutOfRangeException)
            {
                dt = dt.AddMonths(totalToAdd - maxyear * 12);
            }
        }

        private static void AddYears(ref DateTime dt, int count)
        {
            int totalToAdd = count;
            try
            {
                if (Math.Abs(totalToAdd) > 10000) totalToAdd = totalToAdd % 10000;
                if (totalToAdd < 0) totalToAdd += maxyear;
                dt = dt.AddYears(totalToAdd);
            }
            catch (ArgumentOutOfRangeException)
            {
                dt = dt.AddYears(totalToAdd - maxyear);
            }
        }
    }
}
