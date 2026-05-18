using System.Globalization;
using LanguageExt;
using NodaTime;
using NodaTime.Calendars;
using TIKSN.Navigation;

namespace TIKSN.Time;

public readonly struct IsoWeekYear : IYear<IsoWeekYear>
{
    private static readonly IWeekYearRule WeekYearRule = WeekYearRules.Iso;
    private readonly int weekYear;

    public IsoWeekYear(int weekYear) => this.weekYear = YearHelper.EnsureNonZero(weekYear, nameof(weekYear));

    public IsoWeekYear(Era era, int weekYearOfEra)
    {
        ArgumentNullException.ThrowIfNull(era);

        this.weekYear = YearHelper.EnsureNonZero(
            CalendarSystem.Iso.GetAbsoluteYear(weekYearOfEra, era),
            nameof(weekYearOfEra));
    }

    #region Conversion

    public DateInterval ToDateInterval()
    {
        var startDate = WeekYearRule.GetLocalDate(this.weekYear, 1, IsoDayOfWeek.Monday);
        var endDate = WeekYearRule.GetLocalDate(
            this.weekYear,
            WeekYearRule.GetWeeksInWeekYear(this.weekYear),
            IsoDayOfWeek.Sunday);

        return new DateInterval(startDate, endDate);
    }

    #endregion Conversion

    #region Contains

    public bool Contains(LocalDate localDate)
        => this.weekYear == WeekYearRule.GetWeekYear(localDate);

    public bool Contains(LocalDateTime localDateTime)
        => this.Contains(localDateTime.Date);

    public bool Contains(ZonedDateTime zonedDateTime)
        => this.Contains(zonedDateTime.Date);

    public bool Contains(OffsetDateTime offsetDateTime)
        => this.Contains(offsetDateTime.Date);

    public bool Contains(OffsetDate offsetDate)
        => this.Contains(offsetDate.Date);

    public bool Contains(YearMonth yearMonth)
    {
        var yearInterval = this.ToDateInterval();
        var monthStart = yearMonth.OnDayOfMonth(1);
        var monthEnd = monthStart.PlusMonths(1).PlusDays(-1);

        return monthStart <= yearInterval.End && yearInterval.Start <= monthEnd;
    }

    #endregion Contains

    #region Comparison

    public static bool operator !=(IsoWeekYear left, IsoWeekYear right)
        => !(left == right);

    public static bool operator <(IsoWeekYear left, IsoWeekYear right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(IsoWeekYear left, IsoWeekYear right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(IsoWeekYear left, IsoWeekYear right)
        => left.Equals(right);

    public static bool operator >(IsoWeekYear left, IsoWeekYear right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(IsoWeekYear left, IsoWeekYear right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(IsoWeekYear other)
        => this.weekYear.CompareTo(other.weekYear);

    public int CompareTo(object? obj)
    {
        if (obj is IsoWeekYear other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(IsoWeekYear)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is IsoWeekYear other && this.Equals(other);

    public bool Equals(IsoWeekYear other)
        => this.weekYear.Equals(other.weekYear);

    public override int GetHashCode()
        => this.weekYear.GetHashCode();

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        => this.weekYear.ToString(formatProvider);

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var result = this.ToString(new string(format), provider);
        charsWritten = Math.Min(result.Length, destination.Length);
        result.CopyTo(destination[..charsWritten]);
        return charsWritten == result.Length;
    }

    #endregion Formatting

    #region Next and Previous

    public Option<IsoWeekYear> GetNext(int numberOfYears)
        => YearHelper.GetNextYear(this.weekYear, numberOfYears).Map(year => new IsoWeekYear(year));

    public Option<IsoWeekYear> GetNext()
        => this.GetNext(1);

    Option<IYear> ISequentialNavigator<IYear>.GetNext()
        => this.GetNext().Map(year => (IYear)year);

    Option<IYear> IYear.GetNext(int numberOfYears)
        => this.GetNext(numberOfYears).Map(year => (IYear)year);

    public Option<IsoWeekYear> GetPrevious(int numberOfYears)
        => YearHelper.GetPreviousYear(this.weekYear, numberOfYears).Map(year => new IsoWeekYear(year));

    public Option<IsoWeekYear> GetPrevious()
        => this.GetPrevious(1);

    Option<IYear> ISequentialNavigator<IYear>.GetPrevious()
        => this.GetPrevious().Map(year => (IYear)year);

    Option<IYear> IYear.GetPrevious(int numberOfYears)
        => this.GetPrevious(numberOfYears).Map(year => (IYear)year);

    #endregion Next and Previous
}
