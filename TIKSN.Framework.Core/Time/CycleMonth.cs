using System.Globalization;
using LanguageExt;
using NodaTime;
using TIKSN.Navigation;

namespace TIKSN.Time;

public readonly struct CycleMonth : IMonth<CycleMonth>
{
    private readonly int startDayOfMonth;
    private readonly YearMonth startMonth;

    public CycleMonth(YearMonth startMonth, int startDayOfMonth)
    {
        this.startMonth = MonthHelper.EnsureNonZero(startMonth, nameof(startMonth));
        this.startDayOfMonth = MonthHelper.EnsureDayOfMonth(startDayOfMonth, nameof(startDayOfMonth));
    }

    #region Conversion

    public DateInterval ToDateInterval()
    {
        var dayOfMonth = this.startDayOfMonth;
        var startDate = MonthHelper.GetClampedDate(this.startMonth, this.startDayOfMonth);
        var endDate = MonthHelper.GetShiftedYearMonth(this.startMonth, 1)
            .Map(nextStartMonth => MonthHelper.GetClampedDate(nextStartMonth, dayOfMonth).PlusDays(-1))
            .IfNone(() => startDate.PlusMonths(1).PlusDays(-1));

        return new DateInterval(startDate, endDate);
    }

    #endregion Conversion

    internal int GetStartDayOfMonth()
        => this.startDayOfMonth;

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

    public static bool operator !=(CycleMonth left, CycleMonth right)
        => !(left == right);

    public static bool operator <(CycleMonth left, CycleMonth right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(CycleMonth left, CycleMonth right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(CycleMonth left, CycleMonth right)
        => left.Equals(right);

    public static bool operator >(CycleMonth left, CycleMonth right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(CycleMonth left, CycleMonth right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(CycleMonth other)
        => this.ToDateInterval().Start.CompareTo(other.ToDateInterval().Start);

    public int CompareTo(object? obj)
    {
        if (obj is CycleMonth other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(CycleMonth)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is CycleMonth other && this.Equals(other);

    public bool Equals(CycleMonth other)
        => this.startMonth.Equals(other.startMonth) && this.startDayOfMonth == other.startDayOfMonth;

    public override int GetHashCode()
        => HashCode.Combine(this.startMonth, this.startDayOfMonth);

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        => $"{this.startMonth.ToString(format, formatProvider)}-{this.startDayOfMonth.ToString("00", formatProvider)}";

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

    public Option<CycleMonth> GetNext(int numberOfMonths)
    {
        var dayOfMonth = this.startDayOfMonth;

        return MonthHelper.GetShiftedYearMonth(this.startMonth, numberOfMonths)
            .Map(month => new CycleMonth(month, dayOfMonth));
    }

    public Option<CycleMonth> GetNext()
        => this.GetNext(1);

    Option<IMonth> ISequentialNavigator<IMonth>.GetNext()
        => this.GetNext().Map(month => (IMonth)month);

    Option<IMonth> IMonth.GetNext(int numberOfMonths)
        => this.GetNext(numberOfMonths).Map(month => (IMonth)month);

    public Option<CycleMonth> GetPrevious(int numberOfMonths)
    {
        var dayOfMonth = this.startDayOfMonth;

        return MonthHelper.GetShiftedYearMonth(this.startMonth, -1L * numberOfMonths)
            .Map(month => new CycleMonth(month, dayOfMonth));
    }

    public Option<CycleMonth> GetPrevious()
        => this.GetPrevious(1);

    Option<IMonth> ISequentialNavigator<IMonth>.GetPrevious()
        => this.GetPrevious().Map(month => (IMonth)month);

    Option<IMonth> IMonth.GetPrevious(int numberOfMonths)
        => this.GetPrevious(numberOfMonths).Map(month => (IMonth)month);

    #endregion Next and Previous
}
