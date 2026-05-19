using System.Globalization;
using LanguageExt;
using NodaTime;
using TIKSN.Navigation;
using static LanguageExt.Prelude;

namespace TIKSN.Time;

public readonly struct FiscalDay : IDay<FiscalDay>
{
    private readonly int dayOfFiscalYear;
    private readonly FiscalYear fiscalYear;

    public FiscalDay(FiscalYear fiscalYear, int dayOfFiscalYear)
    {
        this.fiscalYear = fiscalYear;
        this.dayOfFiscalYear = DayHelper.EnsureDayInInterval(
            dayOfFiscalYear,
            fiscalYear.ToDateInterval(),
            nameof(dayOfFiscalYear));
    }

    private static FiscalDay Create(LocalDate localDate, FiscalYear fiscalYear)
        => new(fiscalYear, DayHelper.GetDayOfInterval(localDate, fiscalYear.ToDateInterval()));

    private static Option<FiscalDay> FromLocalDate(LocalDate localDate, AnnualDate startDate)
    {
        try
        {
            var year = new FiscalYear(localDate.Year, startDate);
            var interval = year.ToDateInterval();

            if (localDate < interval.Start)
            {
                return YearHelper.GetPreviousYear(localDate.Year, 1)
                    .Map(startYear => Create(localDate, new FiscalYear(startYear, startDate)));
            }

            if (localDate > interval.End)
            {
                return YearHelper.GetNextYear(localDate.Year, 1)
                    .Map(startYear => Create(localDate, new FiscalYear(startYear, startDate)));
            }

            return Create(localDate, year);
        }
        catch (ArgumentOutOfRangeException)
        {
            return None;
        }
    }

    #region Conversion

    public LocalDate ToLocalDate()
        => this.fiscalYear.ToDateInterval().Start.PlusDays(this.dayOfFiscalYear - 1);

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

    public static bool operator !=(FiscalDay left, FiscalDay right)
        => !(left == right);

    public static bool operator <(FiscalDay left, FiscalDay right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(FiscalDay left, FiscalDay right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(FiscalDay left, FiscalDay right)
        => left.Equals(right);

    public static bool operator >(FiscalDay left, FiscalDay right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(FiscalDay left, FiscalDay right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(FiscalDay other)
        => this.ToLocalDate().CompareTo(other.ToLocalDate());

    public int CompareTo(object? obj)
    {
        if (obj is FiscalDay other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(FiscalDay)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is FiscalDay other && this.Equals(other);

    public bool Equals(FiscalDay other)
        => this.fiscalYear.Equals(other.fiscalYear) && this.dayOfFiscalYear == other.dayOfFiscalYear;

    public override int GetHashCode()
        => HashCode.Combine(this.fiscalYear, this.dayOfFiscalYear);

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        => $"{this.fiscalYear.ToString(format, formatProvider)}-{this.dayOfFiscalYear.ToString("00", formatProvider)}";

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

    public Option<FiscalDay> GetNext(int numberOfDays)
    {
        var localDate = this.ToLocalDate();
        var startDate = this.fiscalYear.GetStartDate();

        return DayHelper.GetShiftedLocalDate(localDate, numberOfDays)
            .Bind(day => FromLocalDate(day, startDate));
    }

    public Option<FiscalDay> GetNext()
        => this.GetNext(1);

    Option<IDay> ISequentialNavigator<IDay>.GetNext()
        => this.GetNext().Map(day => (IDay)day);

    Option<IDay> IDay.GetNext(int numberOfDays)
        => this.GetNext(numberOfDays).Map(day => (IDay)day);

    public Option<FiscalDay> GetPrevious(int numberOfDays)
    {
        var localDate = this.ToLocalDate();
        var startDate = this.fiscalYear.GetStartDate();

        return DayHelper.GetShiftedLocalDate(localDate, -1L * numberOfDays)
            .Bind(day => FromLocalDate(day, startDate));
    }

    public Option<FiscalDay> GetPrevious()
        => this.GetPrevious(1);

    Option<IDay> ISequentialNavigator<IDay>.GetPrevious()
        => this.GetPrevious().Map(day => (IDay)day);

    Option<IDay> IDay.GetPrevious(int numberOfDays)
        => this.GetPrevious(numberOfDays).Map(day => (IDay)day);

    #endregion Next and Previous
}
