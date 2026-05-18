using System.Globalization;
using LanguageExt;
using NodaTime;
using NodaTime.Calendars;
using TIKSN.Navigation;

namespace TIKSN.Time;

public readonly struct CalendarMonth : IMonth<CalendarMonth>
{
    private readonly YearMonth yearMonth;

    public CalendarMonth(int year, int month)
        : this(new YearMonth(YearHelper.EnsureNonZero(year, nameof(year)), month))
    {
    }

    public CalendarMonth(int year, int month, CalendarSystem calendar)
        : this(new YearMonth(YearHelper.EnsureNonZero(year, nameof(year)), month, calendar))
    {
    }

    public CalendarMonth(Era era, int yearOfEra, int month)
        : this(era, yearOfEra, month, CalendarSystem.Iso)
    {
    }

    public CalendarMonth(Era era, int yearOfEra, int month, CalendarSystem calendar)
    {
        ArgumentNullException.ThrowIfNull(era);
        ArgumentNullException.ThrowIfNull(calendar);

        this.yearMonth = new YearMonth(
            YearHelper.EnsureNonZero(calendar.GetAbsoluteYear(yearOfEra, era), nameof(yearOfEra)),
            month,
            calendar);
    }

    public CalendarMonth(YearMonth yearMonth)
        => this.yearMonth = MonthHelper.EnsureNonZero(yearMonth, nameof(yearMonth));

    #region Conversion

    public DateInterval ToDateInterval()
    {
        var startDate = this.yearMonth.OnDayOfMonth(1);
        var endDate = startDate.PlusMonths(1).PlusDays(-1);

        return new DateInterval(startDate, endDate);
    }

    #endregion Conversion

    #region Contains

    public bool Contains(LocalDate localDate)
        => MonthHelper.Contains(this.ToDateInterval(), localDate);

    public bool Contains(LocalDateTime localDateTime)
        => this.Contains(localDateTime.Date);

    public bool Contains(ZonedDateTime zonedDateTime)
        => this.Contains(zonedDateTime.Date);

    public bool Contains(OffsetDateTime offsetDateTime)
        => this.Contains(offsetDateTime.Date);

    public bool Contains(OffsetDate offsetDate)
        => this.Contains(offsetDate.Date);

    public bool Contains(YearMonth yearMonth)
        => this.yearMonth.Equals(yearMonth);

    #endregion Contains

    #region Comparison

    public static bool operator !=(CalendarMonth left, CalendarMonth right)
        => !(left == right);

    public static bool operator <(CalendarMonth left, CalendarMonth right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(CalendarMonth left, CalendarMonth right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(CalendarMonth left, CalendarMonth right)
        => left.Equals(right);

    public static bool operator >(CalendarMonth left, CalendarMonth right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(CalendarMonth left, CalendarMonth right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(CalendarMonth other)
        => this.ToDateInterval().Start.CompareTo(other.ToDateInterval().Start);

    public int CompareTo(object? obj)
    {
        if (obj is CalendarMonth other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(CalendarMonth)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is CalendarMonth other && this.Equals(other);

    public bool Equals(CalendarMonth other)
        => this.yearMonth.Equals(other.yearMonth);

    public override int GetHashCode()
        => this.yearMonth.GetHashCode();

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        => this.yearMonth.ToString(format, formatProvider);

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

    public Option<CalendarMonth> GetNext(int numberOfMonths)
        => MonthHelper.GetShiftedYearMonth(this.yearMonth, numberOfMonths).Map(month => new CalendarMonth(month));

    public Option<CalendarMonth> GetNext()
        => this.GetNext(1);

    Option<IMonth> ISequentialNavigator<IMonth>.GetNext()
        => this.GetNext().Map(month => (IMonth)month);

    Option<IMonth> IMonth.GetNext(int numberOfMonths)
        => this.GetNext(numberOfMonths).Map(month => (IMonth)month);

    public Option<CalendarMonth> GetPrevious(int numberOfMonths)
        => MonthHelper.GetShiftedYearMonth(this.yearMonth, -1L * numberOfMonths).Map(month => new CalendarMonth(month));

    public Option<CalendarMonth> GetPrevious()
        => this.GetPrevious(1);

    Option<IMonth> ISequentialNavigator<IMonth>.GetPrevious()
        => this.GetPrevious().Map(month => (IMonth)month);

    Option<IMonth> IMonth.GetPrevious(int numberOfMonths)
        => this.GetPrevious(numberOfMonths).Map(month => (IMonth)month);

    #endregion Next and Previous
}
