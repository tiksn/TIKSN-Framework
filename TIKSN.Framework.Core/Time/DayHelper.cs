using LanguageExt;
using NodaTime;
using static LanguageExt.Prelude;

namespace TIKSN.Time;

internal static class DayHelper
{
    public static int EnsureDayInInterval(int day, DateInterval interval, string paramName)
    {
        if (day < 1)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        try
        {
            if (interval.Start.PlusDays(day - 1) > interval.End)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }
        catch (OverflowException)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        return day;
    }

    public static LocalDate EnsureNonZero(LocalDate localDate, string paramName)
    {
        if (GetAbsoluteYear(localDate) == 0)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        return localDate;
    }

    public static int GetDayOfInterval(LocalDate localDate, DateInterval interval)
        => Period.Between(interval.Start, localDate, PeriodUnits.Days).Days + 1;

    public static Option<LocalDate> GetShiftedLocalDate(LocalDate localDate, long numberOfDays)
    {
        if (numberOfDays is < int.MinValue or > int.MaxValue)
        {
            return None;
        }

        try
        {
            var shiftedDate = localDate.PlusDays((int)numberOfDays);

            return GetAbsoluteYear(shiftedDate) == 0 ? None : shiftedDate;
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

    public static DateInterval ToDateInterval(LocalDate localDate)
        => new(localDate, localDate);

    private static int GetAbsoluteYear(LocalDate localDate)
        => localDate.Calendar.GetAbsoluteYear(localDate.YearOfEra, localDate.Era);
}
