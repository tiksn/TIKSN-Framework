using System.Globalization;
using LanguageExt;
using NodaTime;
using TIKSN.Navigation;
using static LanguageExt.Prelude;

namespace TIKSN.Time;

public readonly struct CycleDay : IDay<CycleDay>
{
    private readonly CycleMonth cycleMonth;
    private readonly int dayOfCycleMonth;

    public CycleDay(CycleMonth cycleMonth, int dayOfCycleMonth)
    {
        this.cycleMonth = cycleMonth;
        this.dayOfCycleMonth = DayHelper.EnsureDayInInterval(
            dayOfCycleMonth,
            cycleMonth.ToDateInterval(),
            nameof(dayOfCycleMonth));
    }

    private static CycleDay Create(LocalDate localDate, CycleMonth cycleMonth)
        => new(cycleMonth, DayHelper.GetDayOfInterval(localDate, cycleMonth.ToDateInterval()));

    private static Option<CycleDay> FromLocalDate(LocalDate localDate, int startDayOfMonth)
    {
        var month = localDate.ToYearMonth();

        try
        {
            var cycleMonth = new CycleMonth(month, startDayOfMonth);
            var interval = cycleMonth.ToDateInterval();

            if (localDate < interval.Start)
            {
                return MonthHelper.GetShiftedYearMonth(month, -1)
                    .Map(previousMonth => Create(localDate, new CycleMonth(previousMonth, startDayOfMonth)));
            }

            if (localDate > interval.End)
            {
                return MonthHelper.GetShiftedYearMonth(month, 1)
                    .Map(nextMonth => Create(localDate, new CycleMonth(nextMonth, startDayOfMonth)));
            }

            return Create(localDate, cycleMonth);
        }
        catch (ArgumentOutOfRangeException)
        {
            return None;
        }
    }

    #region Conversion

    public LocalDate ToLocalDate()
        => this.cycleMonth.ToDateInterval().Start.PlusDays(this.dayOfCycleMonth - 1);

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

    public static bool operator !=(CycleDay left, CycleDay right)
        => !(left == right);

    public static bool operator <(CycleDay left, CycleDay right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(CycleDay left, CycleDay right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(CycleDay left, CycleDay right)
        => left.Equals(right);

    public static bool operator >(CycleDay left, CycleDay right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(CycleDay left, CycleDay right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(CycleDay other)
        => this.ToLocalDate().CompareTo(other.ToLocalDate());

    public int CompareTo(object? obj)
    {
        if (obj is CycleDay other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(CycleDay)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is CycleDay other && this.Equals(other);

    public bool Equals(CycleDay other)
        => this.cycleMonth.Equals(other.cycleMonth) && this.dayOfCycleMonth == other.dayOfCycleMonth;

    public override int GetHashCode()
        => HashCode.Combine(this.cycleMonth, this.dayOfCycleMonth);

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        => $"{this.cycleMonth.ToString(format, formatProvider)}-{this.dayOfCycleMonth.ToString("00", formatProvider)}";

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

    public Option<CycleDay> GetNext(int numberOfDays)
    {
        var localDate = this.ToLocalDate();
        var startDayOfMonth = this.cycleMonth.GetStartDayOfMonth();

        return DayHelper.GetShiftedLocalDate(localDate, numberOfDays)
            .Bind(day => FromLocalDate(day, startDayOfMonth));
    }

    public Option<CycleDay> GetNext()
        => this.GetNext(1);

    Option<IDay> ISequentialNavigator<IDay>.GetNext()
        => this.GetNext().Map(day => (IDay)day);

    Option<IDay> IDay.GetNext(int numberOfDays)
        => this.GetNext(numberOfDays).Map(day => (IDay)day);

    public Option<CycleDay> GetPrevious(int numberOfDays)
    {
        var localDate = this.ToLocalDate();
        var startDayOfMonth = this.cycleMonth.GetStartDayOfMonth();

        return DayHelper.GetShiftedLocalDate(localDate, -1L * numberOfDays)
            .Bind(day => FromLocalDate(day, startDayOfMonth));
    }

    public Option<CycleDay> GetPrevious()
        => this.GetPrevious(1);

    Option<IDay> ISequentialNavigator<IDay>.GetPrevious()
        => this.GetPrevious().Map(day => (IDay)day);

    Option<IDay> IDay.GetPrevious(int numberOfDays)
        => this.GetPrevious(numberOfDays).Map(day => (IDay)day);

    #endregion Next and Previous
}
