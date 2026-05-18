using System.Globalization;
using LanguageExt;
using NodaTime;
using NodaTime.Calendars;
using TIKSN.Navigation;

namespace TIKSN.Time;

public readonly struct CalendarDay : IDay<CalendarDay>
{
    private readonly LocalDate localDate;

    public CalendarDay(int year, int month, int day)
        : this(new LocalDate(YearHelper.EnsureNonZero(year, nameof(year)), month, day))
    {
    }

    public CalendarDay(int year, int month, int day, CalendarSystem calendar)
        : this(new LocalDate(YearHelper.EnsureNonZero(year, nameof(year)), month, day, calendar))
    {
    }

    public CalendarDay(Era era, int yearOfEra, int month, int day)
        : this(era, yearOfEra, month, day, CalendarSystem.Iso)
    {
    }

    public CalendarDay(Era era, int yearOfEra, int month, int day, CalendarSystem calendar)
    {
        ArgumentNullException.ThrowIfNull(era);
        ArgumentNullException.ThrowIfNull(calendar);

        this.localDate = new LocalDate(
            YearHelper.EnsureNonZero(calendar.GetAbsoluteYear(yearOfEra, era), nameof(yearOfEra)),
            month,
            day,
            calendar);
    }

    public CalendarDay(LocalDate localDate)
        => this.localDate = DayHelper.EnsureNonZero(localDate, nameof(localDate));

    #region Conversion

    public LocalDate ToLocalDate()
        => this.localDate;

    public DateInterval ToDateInterval()
        => DayHelper.ToDateInterval(this.localDate);

    #endregion Conversion

    #region Contains

    public bool Contains(LocalDate localDate)
        => this.localDate.Equals(localDate);

    public bool Contains(LocalDateTime localDateTime)
        => this.Contains(localDateTime.Date);

    public bool Contains(ZonedDateTime zonedDateTime)
        => this.Contains(zonedDateTime.Date);

    public bool Contains(OffsetDateTime offsetDateTime)
        => this.Contains(offsetDateTime.Date);

    public bool Contains(OffsetDate offsetDate)
        => this.Contains(offsetDate.Date);

    #endregion Contains

    #region Comparison

    public static bool operator !=(CalendarDay left, CalendarDay right)
        => !(left == right);

    public static bool operator <(CalendarDay left, CalendarDay right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(CalendarDay left, CalendarDay right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(CalendarDay left, CalendarDay right)
        => left.Equals(right);

    public static bool operator >(CalendarDay left, CalendarDay right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(CalendarDay left, CalendarDay right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(CalendarDay other)
        => this.localDate.CompareTo(other.localDate);

    public int CompareTo(object? obj)
    {
        if (obj is CalendarDay other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(CalendarDay)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is CalendarDay other && this.Equals(other);

    public bool Equals(CalendarDay other)
        => this.localDate.Equals(other.localDate);

    public override int GetHashCode()
        => this.localDate.GetHashCode();

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        => this.localDate.ToString(string.IsNullOrEmpty(format) ? "yyyy-MM-dd" : format, formatProvider);

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

    public Option<CalendarDay> GetNext(int numberOfDays)
        => DayHelper.GetShiftedLocalDate(this.localDate, numberOfDays).Map(day => new CalendarDay(day));

    public Option<CalendarDay> GetNext()
        => this.GetNext(1);

    Option<IDay> ISequentialNavigator<IDay>.GetNext()
        => this.GetNext().Map(day => (IDay)day);

    Option<IDay> IDay.GetNext(int numberOfDays)
        => this.GetNext(numberOfDays).Map(day => (IDay)day);

    public Option<CalendarDay> GetPrevious(int numberOfDays)
        => DayHelper.GetShiftedLocalDate(this.localDate, -1L * numberOfDays).Map(day => new CalendarDay(day));

    public Option<CalendarDay> GetPrevious()
        => this.GetPrevious(1);

    Option<IDay> ISequentialNavigator<IDay>.GetPrevious()
        => this.GetPrevious().Map(day => (IDay)day);

    Option<IDay> IDay.GetPrevious(int numberOfDays)
        => this.GetPrevious(numberOfDays).Map(day => (IDay)day);

    #endregion Next and Previous
}
