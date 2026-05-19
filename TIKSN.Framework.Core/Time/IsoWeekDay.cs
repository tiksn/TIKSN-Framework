using System.Globalization;
using LanguageExt;
using NodaTime;
using NodaTime.Calendars;
using TIKSN.Navigation;

namespace TIKSN.Time;

public readonly struct IsoWeekDay : IDay<IsoWeekDay>
{
    private static readonly IWeekYearRule WeekYearRule = WeekYearRules.Iso;
    private readonly IsoDayOfWeek dayOfWeek;
    private readonly int weekOfWeekYear;

    private readonly int weekYear;

    public IsoWeekDay(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
    {
        this.weekYear = YearHelper.EnsureNonZero(weekYear, nameof(weekYear));
        this.weekOfWeekYear = EnsureWeekOfWeekYear(this.weekYear, weekOfWeekYear, nameof(weekOfWeekYear));
        this.dayOfWeek = EnsureDayOfWeek(dayOfWeek, nameof(dayOfWeek));
    }

    public IsoWeekDay(Era era, int weekYearOfEra, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        : this(
            CalendarSystem.Iso.GetAbsoluteYear(weekYearOfEra, era),
            weekOfWeekYear,
            dayOfWeek)
    {
    }

    private static IsoDayOfWeek EnsureDayOfWeek(IsoDayOfWeek dayOfWeek, string paramName)
    {
        if (dayOfWeek is < IsoDayOfWeek.Monday or > IsoDayOfWeek.Sunday)
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        return dayOfWeek;
    }

    private static int EnsureWeekOfWeekYear(int weekYear, int weekOfWeekYear, string paramName)
    {
        if (weekOfWeekYear < 1 || weekOfWeekYear > WeekYearRule.GetWeeksInWeekYear(weekYear))
        {
            throw new ArgumentOutOfRangeException(paramName);
        }

        return weekOfWeekYear;
    }

    private static IsoWeekDay FromLocalDate(LocalDate localDate)
        => new(
            WeekYearRule.GetWeekYear(localDate),
            WeekYearRule.GetWeekOfWeekYear(localDate),
            localDate.DayOfWeek);

    #region Conversion

    public LocalDate ToLocalDate()
        => WeekYearRule.GetLocalDate(this.weekYear, this.weekOfWeekYear, this.dayOfWeek);

    public DateInterval ToDateInterval()
        => DayHelper.ToDateInterval(this.ToLocalDate());

    #endregion Conversion

    #region Contains

    public bool Contains(LocalDate localDate)
        => this.ToLocalDate().Equals(localDate);

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

    public static bool operator !=(IsoWeekDay left, IsoWeekDay right)
        => !(left == right);

    public static bool operator <(IsoWeekDay left, IsoWeekDay right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(IsoWeekDay left, IsoWeekDay right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(IsoWeekDay left, IsoWeekDay right)
        => left.Equals(right);

    public static bool operator >(IsoWeekDay left, IsoWeekDay right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(IsoWeekDay left, IsoWeekDay right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(IsoWeekDay other)
        => this.ToLocalDate().CompareTo(other.ToLocalDate());

    public int CompareTo(object? obj)
    {
        if (obj is IsoWeekDay other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(IsoWeekDay)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is IsoWeekDay other && this.Equals(other);

    public bool Equals(IsoWeekDay other)
        => this.weekYear == other.weekYear
            && this.weekOfWeekYear == other.weekOfWeekYear
            && this.dayOfWeek == other.dayOfWeek;

    public override int GetHashCode()
        => HashCode.Combine(this.weekYear, this.weekOfWeekYear, this.dayOfWeek);

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        =>
            $"{this.weekYear.ToString(formatProvider)}-W{this.weekOfWeekYear.ToString("00", formatProvider)}-{((int)this.dayOfWeek).ToString(formatProvider)}";

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

    public Option<IsoWeekDay> GetNext(int numberOfDays)
        => DayHelper.GetShiftedLocalDate(this.ToLocalDate(), numberOfDays).Map(FromLocalDate);

    public Option<IsoWeekDay> GetNext()
        => this.GetNext(1);

    Option<IDay> ISequentialNavigator<IDay>.GetNext()
        => this.GetNext().Map(day => (IDay)day);

    Option<IDay> IDay.GetNext(int numberOfDays)
        => this.GetNext(numberOfDays).Map(day => (IDay)day);

    public Option<IsoWeekDay> GetPrevious(int numberOfDays)
        => DayHelper.GetShiftedLocalDate(this.ToLocalDate(), -1L * numberOfDays).Map(FromLocalDate);

    public Option<IsoWeekDay> GetPrevious()
        => this.GetPrevious(1);

    Option<IDay> ISequentialNavigator<IDay>.GetPrevious()
        => this.GetPrevious().Map(day => (IDay)day);

    Option<IDay> IDay.GetPrevious(int numberOfDays)
        => this.GetPrevious(numberOfDays).Map(day => (IDay)day);

    #endregion Next and Previous
}
