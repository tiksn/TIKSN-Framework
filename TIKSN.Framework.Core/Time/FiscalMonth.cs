using System.Globalization;
using LanguageExt;
using NodaTime;
using TIKSN.Navigation;

namespace TIKSN.Time;

public readonly struct FiscalMonth : IMonth<FiscalMonth>
{
    private readonly FiscalYear fiscalYear;
    private readonly int monthOfFiscalYear;

    public FiscalMonth(FiscalYear fiscalYear, int monthOfFiscalYear)
    {
        this.fiscalYear = fiscalYear;
        this.monthOfFiscalYear = MonthHelper.EnsureMonthOfYear(monthOfFiscalYear, nameof(monthOfFiscalYear));
    }

    #region Conversion

    public DateInterval ToDateInterval()
    {
        var fiscalYearInterval = this.fiscalYear.ToDateInterval();
        var startDate = fiscalYearInterval.Start.PlusMonths(this.monthOfFiscalYear - 1);
        var endDate = this.monthOfFiscalYear == 12
            ? fiscalYearInterval.End
            : startDate.PlusMonths(1).PlusDays(-1);

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
        => MonthHelper.Contains(this.ToDateInterval(), yearMonth);

    #endregion Contains

    #region Comparison

    public static bool operator !=(FiscalMonth left, FiscalMonth right)
        => !(left == right);

    public static bool operator <(FiscalMonth left, FiscalMonth right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(FiscalMonth left, FiscalMonth right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(FiscalMonth left, FiscalMonth right)
        => left.Equals(right);

    public static bool operator >(FiscalMonth left, FiscalMonth right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(FiscalMonth left, FiscalMonth right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(FiscalMonth other)
        => this.ToDateInterval().Start.CompareTo(other.ToDateInterval().Start);

    public int CompareTo(object? obj)
    {
        if (obj is FiscalMonth other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(FiscalMonth)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is FiscalMonth other && this.Equals(other);

    public bool Equals(FiscalMonth other)
        => this.fiscalYear.Equals(other.fiscalYear) && this.monthOfFiscalYear == other.monthOfFiscalYear;

    public override int GetHashCode()
        => HashCode.Combine(this.fiscalYear, this.monthOfFiscalYear);

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        =>
            $"{this.fiscalYear.ToString(format, formatProvider)}-{this.monthOfFiscalYear.ToString("00", formatProvider)}";

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

    public Option<FiscalMonth> GetNext(int numberOfMonths)
    {
        var shiftedMonth = MonthHelper.GetShiftedMonthOfYear(this.monthOfFiscalYear, numberOfMonths);

        return MonthHelper.GetShiftedYear(
                this.fiscalYear,
                this.monthOfFiscalYear,
                numberOfMonths,
                static (year, years) => year.GetNext(years),
                static (year, years) => year.GetPrevious(years))
            .Map(year => new FiscalMonth(year, shiftedMonth));
    }

    public Option<FiscalMonth> GetNext()
        => this.GetNext(1);

    Option<IMonth> ISequentialNavigator<IMonth>.GetNext()
        => this.GetNext().Map(month => (IMonth)month);

    Option<IMonth> IMonth.GetNext(int numberOfMonths)
        => this.GetNext(numberOfMonths).Map(month => (IMonth)month);

    public Option<FiscalMonth> GetPrevious(int numberOfMonths)
    {
        var shiftedMonth = MonthHelper.GetShiftedMonthOfYear(this.monthOfFiscalYear, -1L * numberOfMonths);

        return MonthHelper.GetShiftedYear(
                this.fiscalYear,
                this.monthOfFiscalYear,
                -1L * numberOfMonths,
                static (year, years) => year.GetNext(years),
                static (year, years) => year.GetPrevious(years))
            .Map(year => new FiscalMonth(year, shiftedMonth));
    }

    public Option<FiscalMonth> GetPrevious()
        => this.GetPrevious(1);

    Option<IMonth> ISequentialNavigator<IMonth>.GetPrevious()
        => this.GetPrevious().Map(month => (IMonth)month);

    Option<IMonth> IMonth.GetPrevious(int numberOfMonths)
        => this.GetPrevious(numberOfMonths).Map(month => (IMonth)month);

    #endregion Next and Previous
}
