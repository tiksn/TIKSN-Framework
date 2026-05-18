using LanguageExt;
using NodaTime;
using static LanguageExt.Prelude;

namespace TIKSN.Time;

internal static class MonthHelper
{
    public static bool Contains(DateInterval interval, LocalDate localDate)
        => interval.Start <= localDate && localDate <= interval.End;

    public static bool Contains(DateInterval interval, YearMonth yearMonth)
    {
        var monthStart = yearMonth.OnDayOfMonth(1);
        var monthEnd = monthStart.PlusMonths(1).PlusDays(-1);

        return monthStart <= interval.End && interval.Start <= monthEnd;
    }

    public static int EnsureDayOfMonth(int dayOfMonth, string paramName)
    {
        if (dayOfMonth is < 1 or > 31)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        return dayOfMonth;
    }

    public static int EnsureMonthOfYear(int monthOfYear, string paramName)
    {
        if (monthOfYear is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        return monthOfYear;
    }

    public static YearMonth EnsureNonZero(YearMonth yearMonth, string paramName)
    {
        if (GetAbsoluteYear(yearMonth) == 0)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        return yearMonth;
    }

    public static LocalDate GetClampedDate(YearMonth yearMonth, int dayOfMonth)
    {
        var day = EnsureDayOfMonth(dayOfMonth, nameof(dayOfMonth));

        while (true)
        {
            try
            {
                return yearMonth.OnDayOfMonth(day);
            }
            catch (ArgumentOutOfRangeException) when (day > 1)
            {
                day--;
            }
        }
    }

    public static int GetShiftedMonthOfYear(int monthOfYear, long numberOfMonths)
    {
        var zeroBasedMonth = monthOfYear - 1L;
        var shiftedMonth = zeroBasedMonth + numberOfMonths;
        var newZeroBasedMonth = shiftedMonth % 12;

        if (newZeroBasedMonth < 0)
        {
            newZeroBasedMonth += 12;
        }

        return (int)newZeroBasedMonth + 1;
    }

    public static Option<TYear> GetShiftedYear<TYear>(
        TYear year,
        int monthOfYear,
        long numberOfMonths,
        Func<TYear, int, Option<TYear>> getNextYears,
        Func<TYear, int, Option<TYear>> getPreviousYears)
    {
        var zeroBasedMonth = monthOfYear - 1L;
        var shiftedMonth = zeroBasedMonth + numberOfMonths;
        var yearShift = Math.DivRem(shiftedMonth, 12L, out var newZeroBasedMonth);

        if (newZeroBasedMonth < 0)
        {
            yearShift--;
        }

        if (yearShift is < int.MinValue or > int.MaxValue)
        {
            return None;
        }

        if (yearShift > 0)
        {
            return getNextYears(year, (int)yearShift);
        }

        return yearShift < 0 ? getPreviousYears(year, (int)-yearShift) : year;
    }

    public static Option<YearMonth> GetShiftedYearMonth(YearMonth yearMonth, long numberOfMonths)
    {
        if (numberOfMonths is < int.MinValue or > int.MaxValue)
        {
            return None;
        }

        try
        {
            var shiftedYearMonth = yearMonth.PlusMonths((int)numberOfMonths);

            return GetAbsoluteYear(shiftedYearMonth) == 0 ? None : shiftedYearMonth;
        }
        catch (ArgumentOutOfRangeException)
        {
            return None;
        }
        catch (OverflowException)
        {
            return None;
        }
    }

    private static int GetAbsoluteYear(YearMonth yearMonth)
        => yearMonth.Calendar.GetAbsoluteYear(yearMonth.YearOfEra, yearMonth.Era);
}
