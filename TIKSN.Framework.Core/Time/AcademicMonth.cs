using System.Globalization;
using LanguageExt;
using NodaTime;
using TIKSN.Navigation;

namespace TIKSN.Time;

public readonly struct AcademicMonth : IMonth<AcademicMonth>
{
    private readonly AcademicYear academicYear;
    private readonly int monthOfAcademicYear;

    public AcademicMonth(AcademicYear academicYear, int monthOfAcademicYear)
    {
        this.academicYear = academicYear;
        this.monthOfAcademicYear = MonthHelper.EnsureMonthOfYear(monthOfAcademicYear, nameof(monthOfAcademicYear));
    }

    #region Conversion

    public DateInterval ToDateInterval()
    {
        var academicYearInterval = this.academicYear.ToDateInterval();
        var startDate = academicYearInterval.Start.PlusMonths(this.monthOfAcademicYear - 1);
        var endDate = this.monthOfAcademicYear == 12
            ? academicYearInterval.End
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

    public static bool operator !=(AcademicMonth left, AcademicMonth right)
        => !(left == right);

    public static bool operator <(AcademicMonth left, AcademicMonth right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(AcademicMonth left, AcademicMonth right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(AcademicMonth left, AcademicMonth right)
        => left.Equals(right);

    public static bool operator >(AcademicMonth left, AcademicMonth right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(AcademicMonth left, AcademicMonth right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(AcademicMonth other)
        => this.ToDateInterval().Start.CompareTo(other.ToDateInterval().Start);

    public int CompareTo(object? obj)
    {
        if (obj is AcademicMonth other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(AcademicMonth)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is AcademicMonth other && this.Equals(other);

    public bool Equals(AcademicMonth other)
        => this.academicYear.Equals(other.academicYear) && this.monthOfAcademicYear == other.monthOfAcademicYear;

    public override int GetHashCode()
        => HashCode.Combine(this.academicYear, this.monthOfAcademicYear);

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        => $"{this.academicYear.ToString(format, formatProvider)}-{this.monthOfAcademicYear.ToString("00", formatProvider)}";

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

    public Option<AcademicMonth> GetNext(int numberOfMonths)
    {
        var shiftedMonth = MonthHelper.GetShiftedMonthOfYear(this.monthOfAcademicYear, numberOfMonths);

        return MonthHelper.GetShiftedYear(
            this.academicYear,
            this.monthOfAcademicYear,
            numberOfMonths,
            static (year, years) => year.GetNext(years),
            static (year, years) => year.GetPrevious(years))
            .Map(year => new AcademicMonth(year, shiftedMonth));
    }

    public Option<AcademicMonth> GetNext()
        => this.GetNext(1);

    Option<IMonth> ISequentialNavigator<IMonth>.GetNext()
        => this.GetNext().Map(month => (IMonth)month);

    Option<IMonth> IMonth.GetNext(int numberOfMonths)
        => this.GetNext(numberOfMonths).Map(month => (IMonth)month);

    public Option<AcademicMonth> GetPrevious(int numberOfMonths)
    {
        var shiftedMonth = MonthHelper.GetShiftedMonthOfYear(this.monthOfAcademicYear, -1L * numberOfMonths);

        return MonthHelper.GetShiftedYear(
            this.academicYear,
            this.monthOfAcademicYear,
            -1L * numberOfMonths,
            static (year, years) => year.GetNext(years),
            static (year, years) => year.GetPrevious(years))
            .Map(year => new AcademicMonth(year, shiftedMonth));
    }

    public Option<AcademicMonth> GetPrevious()
        => this.GetPrevious(1);

    Option<IMonth> ISequentialNavigator<IMonth>.GetPrevious()
        => this.GetPrevious().Map(month => (IMonth)month);

    Option<IMonth> IMonth.GetPrevious(int numberOfMonths)
        => this.GetPrevious(numberOfMonths).Map(month => (IMonth)month);

    #endregion Next and Previous
}
