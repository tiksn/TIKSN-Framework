using System.Globalization;
using System.Runtime.InteropServices;
using LanguageExt;
using NodaTime;
using NodaTime.Calendars;
using TIKSN.Navigation;

namespace TIKSN.Time;

[StructLayout(LayoutKind.Auto)]
public readonly struct FiscalYear : IYear<FiscalYear>
{
    private readonly int absoluteStartYear;
    private readonly AnnualDate startDate;

    public FiscalYear(int startYear, AnnualDate startDate)
        : this(Era.Common, startYear, startDate, CalendarSystem.Iso)
    {
    }

    public FiscalYear(int year, AnnualDate startDate, CalendarSystem calendar)
        : this(Era.Common, year, startDate, calendar)
    {
    }

    public FiscalYear(Era era, int startYearOfEra, AnnualDate startDate)
        : this(era, startYearOfEra, startDate, CalendarSystem.Iso)
    {
    }

    public FiscalYear(Era era, int startYearOfEra, AnnualDate startDate, CalendarSystem calendar)
    {
        ArgumentNullException.ThrowIfNull(era);

        ArgumentNullException.ThrowIfNull(calendar);

        this.absoluteStartYear = YearHelper.EnsureNonZero(
            calendar.GetAbsoluteYear(startYearOfEra, era),
            nameof(startYearOfEra));
        this.startDate = startDate;
    }

    #region Conversion

    public DateInterval ToDateInterval()
    {
        var startDay = this.startDate.InYear(this.absoluteStartYear);
        var endDay = startDay.PlusYears(1).PlusDays(-1);
        return new DateInterval(startDay, endDay);
    }

    #endregion Conversion

    internal AnnualDate GetStartDate()
        => this.startDate;

    #region Contains

    public bool Contains(LocalDate localDate)
    {
        var startLocalDate = this.startDate.InYear(this.absoluteStartYear);
        var nextStartLocalDate = this.startDate.InYear(this.absoluteStartYear + 1);

        return startLocalDate <= localDate && localDate < nextStartLocalDate;
    }

    public bool Contains(LocalDateTime localDateTime)
    {
        var startLocalDate = this.startDate.InYear(this.absoluteStartYear);
        var nextStartLocalDate = this.startDate.InYear(this.absoluteStartYear + 1);

        return startLocalDate <= localDateTime.Date && localDateTime.Date < nextStartLocalDate;
    }

    public bool Contains(ZonedDateTime zonedDateTime)
    {
        var startLocalDate = this.startDate.InYear(this.absoluteStartYear);
        var nextStartLocalDate = this.startDate.InYear(this.absoluteStartYear + 1);

        return startLocalDate <= zonedDateTime.Date && zonedDateTime.Date < nextStartLocalDate;
    }

    public bool Contains(OffsetDateTime offsetDateTime)
    {
        var startLocalDate = this.startDate.InYear(this.absoluteStartYear);
        var nextStartLocalDate = this.startDate.InYear(this.absoluteStartYear + 1);

        return startLocalDate <= offsetDateTime.Date && offsetDateTime.Date < nextStartLocalDate;
    }

    public bool Contains(OffsetDate offsetDate)
    {
        var startLocalDate = this.startDate.InYear(this.absoluteStartYear);
        var nextStartLocalDate = this.startDate.InYear(this.absoluteStartYear + 1);

        return startLocalDate <= offsetDate.Date && offsetDate.Date < nextStartLocalDate;
    }

    public bool Contains(YearMonth yearMonth)
    {
        var startYearMonth = this.startDate.InYear(this.absoluteStartYear).ToYearMonth();
        var nextStartYearMonth = this.startDate.InYear(this.absoluteStartYear + 1).ToYearMonth();

        return startYearMonth <= yearMonth && yearMonth < nextStartYearMonth;
    }

    #endregion Contains

    #region Comparison

    public static bool operator !=(FiscalYear left, FiscalYear right)
        => !(left == right);

    public static bool operator <(FiscalYear left, FiscalYear right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(FiscalYear left, FiscalYear right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(FiscalYear left, FiscalYear right)
        => left.Equals(right);

    public static bool operator >(FiscalYear left, FiscalYear right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(FiscalYear left, FiscalYear right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(FiscalYear other)
        => this.startDate.InYear(this.absoluteStartYear).CompareTo(other.startDate.InYear(other.absoluteStartYear));

    public int CompareTo(object? obj)
    {
        if (obj is FiscalYear other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(FiscalYear)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is FiscalYear other && this.Equals(other);

    public bool Equals(FiscalYear other)
        => this.startDate.InYear(this.absoluteStartYear).Equals(other.startDate.InYear(other.absoluteStartYear));

    public override int GetHashCode()
        => HashCode.Combine(this.absoluteStartYear, this.startDate);

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        => $"{this.absoluteStartYear.ToString(formatProvider)}/{(this.absoluteStartYear + 1).ToString(formatProvider)}";

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

    public Option<FiscalYear> GetNext(int numberOfYears)
    {
        var fiscalYearStartDate = this.startDate;

        return YearHelper.GetNextYear(this.absoluteStartYear, numberOfYears)
            .Map(year => new FiscalYear(year, fiscalYearStartDate));
    }

    public Option<FiscalYear> GetNext()
        => this.GetNext(1);

    Option<IYear> ISequentialNavigator<IYear>.GetNext()
        => this.GetNext().Map(year => (IYear)year);

    public Option<FiscalYear> GetPrevious(int numberOfYears)
    {
        var fiscalYearStartDate = this.startDate;

        return YearHelper.GetPreviousYear(this.absoluteStartYear, numberOfYears)
            .Map(year => new FiscalYear(year, fiscalYearStartDate));
    }

    public Option<FiscalYear> GetPrevious()
        => this.GetPrevious(1);

    Option<IYear> IYear.GetNext(int numberOfYears)
        => this.GetNext(numberOfYears).Map(year => (IYear)year);

    Option<IYear> ISequentialNavigator<IYear>.GetPrevious()
        => this.GetPrevious().Map(year => (IYear)year);

    Option<IYear> IYear.GetPrevious(int numberOfYears)
        => this.GetPrevious(numberOfYears).Map(year => (IYear)year);

    #endregion Next and Previous
}
