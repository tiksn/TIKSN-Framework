using System.Globalization;
using NodaTime;
using NodaTime.Calendars;

namespace TIKSN.Time;

public readonly struct AcademicYear : IYear<AcademicYear>
{
    private static readonly AnnualDate StartDate = new AnnualDate(9, 1);
    private readonly int absoluteStartYear;

    public AcademicYear(int startYear)
        : this(Era.Common, startYear, CalendarSystem.Iso)
    {
    }

    public AcademicYear(int year, CalendarSystem calendar)
        : this(Era.Common, year, calendar)
    {
    }

    public AcademicYear(Era era, int startYearOfEra)
        : this(era, startYearOfEra, CalendarSystem.Iso)
    {
    }

    public AcademicYear(Era era, int startYearOfEra, CalendarSystem calendar)
    {
        if (era is null)
        {
            throw new ArgumentNullException(nameof(era));
        }

        if (calendar is null)
        {
            throw new ArgumentNullException(nameof(calendar));
        }

        this.absoluteStartYear = calendar.GetAbsoluteYear(startYearOfEra, era);
    }

    #region Contains

    public bool Contains(LocalDate localDate)
    {
        var startLocalDate = StartDate.InYear(this.absoluteStartYear);
        var nextStartLocalDate = StartDate.InYear(this.absoluteStartYear + 1);

        return startLocalDate <= localDate && localDate < nextStartLocalDate;
    }

    public bool Contains(LocalDateTime localDateTime)
    {
        var startLocalDate = StartDate.InYear(this.absoluteStartYear);
        var nextStartLocalDate = StartDate.InYear(this.absoluteStartYear + 1);

        return startLocalDate <= localDateTime.Date && localDateTime.Date < nextStartLocalDate;
    }

    public bool Contains(ZonedDateTime zonedDateTime)
    {
        var startLocalDate = StartDate.InYear(this.absoluteStartYear);
        var nextStartLocalDate = StartDate.InYear(this.absoluteStartYear + 1);

        return startLocalDate <= zonedDateTime.Date && zonedDateTime.Date < nextStartLocalDate;
    }

    public bool Contains(OffsetDateTime offsetDateTime)
    {
        var startLocalDate = StartDate.InYear(this.absoluteStartYear);
        var nextStartLocalDate = StartDate.InYear(this.absoluteStartYear + 1);

        return startLocalDate <= offsetDateTime.Date && offsetDateTime.Date < nextStartLocalDate;
    }

    public bool Contains(OffsetDate offsetDate)
    {
        var startLocalDate = StartDate.InYear(this.absoluteStartYear);
        var nextStartLocalDate = StartDate.InYear(this.absoluteStartYear + 1);

        return startLocalDate <= offsetDate.Date && offsetDate.Date < nextStartLocalDate;
    }

    public bool Contains(YearMonth yearMonth)
    {
        var startYearMonth = StartDate.InYear(this.absoluteStartYear).ToYearMonth();
        var nextStartYearMonth = StartDate.InYear(this.absoluteStartYear + 1).ToYearMonth();

        return startYearMonth <= yearMonth && yearMonth < nextStartYearMonth;
    }

    #endregion Contains

    #region Comparison

    public static bool operator !=(AcademicYear left, AcademicYear right)
        => !(left == right);

    public static bool operator <(AcademicYear left, AcademicYear right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(AcademicYear left, AcademicYear right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(AcademicYear left, AcademicYear right)
        => left.Equals(right);

    public static bool operator >(AcademicYear left, AcademicYear right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(AcademicYear left, AcademicYear right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(AcademicYear other)
        => StartDate.InYear(this.absoluteStartYear).CompareTo(StartDate.InYear(other.absoluteStartYear));

    public int CompareTo(object obj)
    {
        if (obj is AcademicYear other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(AcademicYear)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object obj)
        => obj is AcademicYear other && this.Equals(other);

    public bool Equals(AcademicYear other)
        => StartDate.InYear(this.absoluteStartYear).Equals(StartDate.InYear(other.absoluteStartYear));

    public override int GetHashCode()
        => HashCode.Combine(this.absoluteStartYear, StartDate);

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string format, IFormatProvider formatProvider)
        => $"{this.absoluteStartYear.ToString(formatProvider)}/{(this.absoluteStartYear + 1).ToString(formatProvider)}";

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

    public AcademicYear GetNext(int numberOfYears)
        => new(this.absoluteStartYear + numberOfYears);

    IYear IYear.GetNext(int numberOfYears)
        => this.GetNext(numberOfYears);

    public AcademicYear GetNext()
        => this.GetNext(1);

    IYear IYear.GetNext()
        => this.GetNext(1);

    public AcademicYear GetPrevious(int numberOfYears)
        => new(this.absoluteStartYear - numberOfYears);

    IYear IYear.GetPrevious(int numberOfYears)
        => this.GetPrevious(numberOfYears);

    public AcademicYear GetPrevious()
        => this.GetPrevious(1);

    IYear IYear.GetPrevious()
        => this.GetPrevious(1);

    #endregion Next and Previous

    #region Conversion

    public DateInterval ToDateInterval()
    {
        var startDay = StartDate.InYear(this.absoluteStartYear);
        var endDay = startDay.PlusYears(1).PlusDays(-1);
        return new DateInterval(startDay, endDay);
    }

    #endregion Conversion
}
