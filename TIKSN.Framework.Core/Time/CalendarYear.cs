using System.Globalization;
using NodaTime;
using NodaTime.Calendars;

namespace TIKSN.Time;

public readonly struct CalendarYear : IYear<CalendarYear>
{
    private readonly int absoluteYear;

    public CalendarYear(int year) : this(Era.Common, year, CalendarSystem.Iso)
    {
    }

    public CalendarYear(int year, CalendarSystem calendar) : this(Era.Common, year, calendar)
    {
    }

    public CalendarYear(Era era, int yearOfEra) : this(era, yearOfEra, CalendarSystem.Iso)
    {
    }

    public CalendarYear(Era era, int yearOfEra, CalendarSystem calendar)
    {
        if (era is null)
        {
            throw new ArgumentNullException(nameof(era));
        }

        if (calendar is null)
        {
            throw new ArgumentNullException(nameof(calendar));
        }

        this.absoluteYear = calendar.GetAbsoluteYear(yearOfEra, era);
    }

    #region Contains

    public bool Contains(LocalDate localDate)
        => this.absoluteYear == localDate.Calendar.GetAbsoluteYear(localDate.YearOfEra, localDate.Era);

    public bool Contains(LocalDateTime localDateTime)
        => this.absoluteYear == localDateTime.Calendar.GetAbsoluteYear(localDateTime.YearOfEra, localDateTime.Era);

    public bool Contains(ZonedDateTime zonedDateTime)
        => this.absoluteYear == zonedDateTime.Calendar.GetAbsoluteYear(zonedDateTime.YearOfEra, zonedDateTime.Era);

    public bool Contains(OffsetDateTime offsetDateTime)
        => this.absoluteYear == offsetDateTime.Calendar.GetAbsoluteYear(offsetDateTime.YearOfEra, offsetDateTime.Era);

    public bool Contains(OffsetDate offsetDate)
        => this.absoluteYear == offsetDate.Calendar.GetAbsoluteYear(offsetDate.YearOfEra, offsetDate.Era);

    public bool Contains(YearMonth yearMonth)
        => this.absoluteYear == yearMonth.Calendar.GetAbsoluteYear(yearMonth.YearOfEra, yearMonth.Era);

    #endregion Contains

    #region Comparison

    public static bool operator !=(CalendarYear left, CalendarYear right)
        => !(left == right);

    public static bool operator <(CalendarYear left, CalendarYear right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(CalendarYear left, CalendarYear right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(CalendarYear left, CalendarYear right)
        => left.Equals(right);

    public static bool operator >(CalendarYear left, CalendarYear right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(CalendarYear left, CalendarYear right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(CalendarYear other)
        => this.absoluteYear.CompareTo(other.absoluteYear);

    public int CompareTo(object obj)
    {
        if (obj is CalendarYear other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(CalendarYear)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object obj)
        => obj is CalendarYear other && this.Equals(other);

    public bool Equals(CalendarYear other)
        => this.absoluteYear.Equals(other.absoluteYear);

    public override int GetHashCode()
        => this.absoluteYear.GetHashCode();

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string format, IFormatProvider formatProvider)
        => this.absoluteYear.ToString(formatProvider);

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider provider)
    {
        var result = this.ToString(new string(format), provider);
        charsWritten = Math.Min(result.Length, destination.Length);
        result.CopyTo(destination[..charsWritten]);
        return charsWritten == result.Length;
    }

    #endregion Formatting

    #region Next and Previous

    public CalendarYear GetNext(int numberOfYears)
        => new(this.absoluteYear + numberOfYears);

    IYear IYear.GetNext(int numberOfYears)
        => this.GetNext(numberOfYears);

    public CalendarYear GetNext()
        => this.GetNext(1);

    IYear IYear.GetNext()
        => this.GetNext(1);

    public CalendarYear GetPrevious(int numberOfYears)
        => new(this.absoluteYear - numberOfYears);

    IYear IYear.GetPrevious(int numberOfYears)
        => this.GetPrevious(numberOfYears);

    public CalendarYear GetPrevious()
        => this.GetPrevious(1);

    IYear IYear.GetPrevious()
        => this.GetPrevious(1);

    #endregion Next and Previous

    #region Conversion

    public DateInterval ToDateInterval()
    {
        var startDate = new LocalDate(this.absoluteYear, 1, 1);
        var endDate = new LocalDate(this.absoluteYear, 12, 31);
        return new DateInterval(startDate, endDate);
    }

    #endregion Conversion
}
