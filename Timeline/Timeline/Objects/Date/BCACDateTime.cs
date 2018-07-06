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
        const Int64 TICKS_PER_HOUR = 36000000000;
        const Int64 TICKS_PER_DAY = 864000000000;
        const int MAX_YEARS = 9999;

        protected DateTime? bcacDate { get; set; }
        protected BCAC bcac { get; set; }
        protected int rawYear
        {
            get
            {
                if (bcacDate == null) return 0;
                if (bcac == BCAC.AC)
                    return bcacDate.Value.Year;
                else
                    return -10000 + bcacDate.Value.Year;
            }
        }

        public BCACDateTime() : this(null) { }
        public BCACDateTime(DateTime? dateTime, BCAC bc_or_ac = BCAC.AC)
        {
            bcac = bc_or_ac;
            if (bcac == BCAC.BC)
                bcacDate = new DateTime(10000 - dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day);
            else
                bcacDate = dateTime;
        }

        public virtual void AddMinutes(int count)
        {
            try
            {
                bcacDate = bcacDate?.AddMinutes(count);
                BCACDateChanged();
            }
            catch (ArgumentOutOfRangeException)
            {
                if (bcac == BCAC.AC && count > 0) throw new OverflowException();
                if (bcac == BCAC.BC && count < 0) throw new OverflowException();

                Int64 ticks = bcacDate.Value.Ticks + count * TICKS_PER_MINUTE;
                if (ticks < 0) ticks += (DateTime.MaxValue.Ticks + 1);
                if (ticks > DateTime.MaxValue.Ticks) ticks -= (DateTime.MaxValue.Ticks + 1);
                bcac = count > 0 ? BCAC.AC : BCAC.BC;
                bcacDate = new DateTime(ticks);
                BCACDateChanged();
            }
        }

        public virtual void AddHours(int count)
        {
            try
            {
                bcacDate = bcacDate?.AddHours(count);
                BCACDateChanged();
            }
            catch (ArgumentOutOfRangeException)
            {
                if (bcac == BCAC.AC && count > 0) throw new OverflowException();
                if (bcac == BCAC.BC && count < 0) throw new OverflowException();

                Int64 ticks = bcacDate.Value.Ticks + count * TICKS_PER_HOUR;
                if (ticks < 0) ticks += (DateTime.MaxValue.Ticks + 1);
                if (ticks > DateTime.MaxValue.Ticks) ticks -= (DateTime.MaxValue.Ticks + 1);
                bcac = count > 0 ? BCAC.AC : BCAC.BC;
                bcacDate = new DateTime(ticks);
                BCACDateChanged();
            }
        }

        public virtual void AddDays(int count)
        {
            try
            {
                bcacDate = bcacDate?.AddDays(count);
                BCACDateChanged();
            }
            catch (ArgumentOutOfRangeException)
            {
                if (bcac == BCAC.AC && count > 0) throw new OverflowException();
                if (bcac == BCAC.BC && count < 0) throw new OverflowException();

                Int64 ticks = bcacDate.Value.Ticks + count * TICKS_PER_DAY;
                if (ticks < 0) ticks += (DateTime.MaxValue.Ticks + 1);
                if (ticks > DateTime.MaxValue.Ticks) ticks -= (DateTime.MaxValue.Ticks + 1);
                bcac = count > 0 ? BCAC.AC : BCAC.BC;
                bcacDate = new DateTime(ticks);
                BCACDateChanged();
            }
        }

        public virtual void AddMonths(int count)
        {
            try
            {
                bcacDate = bcacDate?.AddMonths(count);
                BCACDateChanged();
            }
            catch (ArgumentOutOfRangeException)
            {
                if (bcac == BCAC.AC && count > 0) throw new OverflowException();
                if (bcac == BCAC.BC && count < 0) throw new OverflowException();

                bcac = count > 0 ? BCAC.AC : BCAC.BC;
                if (count > 0) bcacDate = bcacDate?.AddMonths(count - MAX_YEARS * 12);
                if (count < 0) bcacDate = bcacDate?.AddMonths(MAX_YEARS * 12 + count);
                BCACDateChanged();
            }
        }

        public virtual void AddYears(int count)
        {
            try
            {
                bcacDate = bcacDate?.AddYears(count);
                BCACDateChanged();
            }
            catch (ArgumentOutOfRangeException)
            {
                if (bcac == BCAC.AC && count > 0) throw new OverflowException();
                if (bcac == BCAC.BC && count >= 2 * MAX_YEARS - bcacDate.Value.Year) throw new OverflowException();
                if (bcac == BCAC.BC && count < 0) throw new OverflowException();
                if (bcac == BCAC.AC && count <= -MAX_YEARS - bcacDate.Value.Year) throw new OverflowException();

                bcac = count > 0 ? BCAC.AC : BCAC.BC;
                if (count > 0) bcacDate = bcacDate?.AddYears(count - MAX_YEARS);
                if (count < 0) bcacDate = bcacDate?.AddYears(MAX_YEARS + count);
                BCACDateChanged();
            }
        }

        protected virtual void BCACDateChanged() { }

    }
}
