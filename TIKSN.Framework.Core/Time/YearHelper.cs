using LanguageExt;
using static LanguageExt.Prelude;

namespace TIKSN.Time;

internal static class YearHelper
{
    public static int EnsureNonZero(int year, string paramName)
    {
        if (year == 0)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        return year;
    }

    public static Option<int> GetNextYear(int year, int numberOfYears)
        => GetShiftedYear(year, numberOfYears);

    public static Option<int> GetPreviousYear(int year, int numberOfYears)
        => GetShiftedYear(year, -1L * numberOfYears);

    private static Option<int> GetShiftedYear(int year, long numberOfYears)
    {
        var shiftedYear = year + numberOfYears;

        if (shiftedYear is < int.MinValue or > int.MaxValue or 0)
        {
            return None;
        }

        return (int)shiftedYear;
    }
}
