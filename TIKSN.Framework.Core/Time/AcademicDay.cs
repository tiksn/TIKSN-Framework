using System.Globalization;
using LanguageExt;
using NodaTime;
using TIKSN.Navigation;
using static LanguageExt.Prelude;

namespace TIKSN.Time;

public readonly struct AcademicDay : IDay<AcademicDay>
{
    private readonly AcademicYear academicYear;
    private readonly int dayOfAcademicYear;

    public AcademicDay(AcademicYear academicYear, int dayOfAcademicYear)
    {
        this.academicYear = academicYear;
        this.dayOfAcademicYear = DayHelper.EnsureDayInInterval(
            dayOfAcademicYear,
            academicYear.ToDateInterval(),
            nameof(dayOfAcademicYear));
    }

    private static AcademicDay Create(LocalDate localDate, AcademicYear academicYear)
        => new(academicYear, DayHelper.GetDayOfInterval(localDate, academicYear.ToDateInterval()));

    private static Option<AcademicDay> FromLocalDate(LocalDate localDate)
    {
        try
        {
            var year = new AcademicYear(localDate.Year);
            var interval = year.ToDateInterval();

            if (localDate < interval.Start)
            {
                return YearHelper.GetPreviousYear(localDate.Year, 1)
                    .Map(startYear => Create(localDate, new AcademicYear(startYear)));
            }

            if (localDate > interval.End)
            {
                return YearHelper.GetNextYear(localDate.Year, 1)
                    .Map(startYear => Create(localDate, new AcademicYear(startYear)));
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
        => this.academicYear.ToDateInterval().Start.PlusDays(this.dayOfAcademicYear - 1);

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

    public static bool operator !=(AcademicDay left, AcademicDay right)
        => !(left == right);

    public static bool operator <(AcademicDay left, AcademicDay right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(AcademicDay left, AcademicDay right)
        => left.CompareTo(right) <= 0;

    public static bool operator ==(AcademicDay left, AcademicDay right)
        => left.Equals(right);

    public static bool operator >(AcademicDay left, AcademicDay right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(AcademicDay left, AcademicDay right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(AcademicDay other)
        => this.ToLocalDate().CompareTo(other.ToLocalDate());

    public int CompareTo(object? obj)
    {
        if (obj is AcademicDay other)
        {
            return this.CompareTo(other);
        }

        throw new ArgumentException($"Object must be of type {typeof(AcademicDay)}.", nameof(obj));
    }

    #endregion Comparison

    #region Equality

    public override bool Equals(object? obj)
        => obj is AcademicDay other && this.Equals(other);

    public bool Equals(AcademicDay other)
        => this.academicYear.Equals(other.academicYear) && this.dayOfAcademicYear == other.dayOfAcademicYear;

    public override int GetHashCode()
        => HashCode.Combine(this.academicYear, this.dayOfAcademicYear);

    #endregion Equality

    #region Formatting

    public override string ToString()
        => this.ToString(string.Empty, CultureInfo.InvariantCulture);

    public string ToString(string? format, IFormatProvider? formatProvider)
        =>
            $"{this.academicYear.ToString(format, formatProvider)}-{this.dayOfAcademicYear.ToString("00", formatProvider)}";

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

    public Option<AcademicDay> GetNext(int numberOfDays)
        => DayHelper.GetShiftedLocalDate(this.ToLocalDate(), numberOfDays).Bind(FromLocalDate);

    public Option<AcademicDay> GetNext()
        => this.GetNext(1);

    Option<IDay> ISequentialNavigator<IDay>.GetNext()
        => this.GetNext().Map(day => (IDay)day);

    Option<IDay> IDay.GetNext(int numberOfDays)
        => this.GetNext(numberOfDays).Map(day => (IDay)day);

    public Option<AcademicDay> GetPrevious(int numberOfDays)
        => DayHelper.GetShiftedLocalDate(this.ToLocalDate(), -1L * numberOfDays).Bind(FromLocalDate);

    public Option<AcademicDay> GetPrevious()
        => this.GetPrevious(1);

    Option<IDay> ISequentialNavigator<IDay>.GetPrevious()
        => this.GetPrevious().Map(day => (IDay)day);

    Option<IDay> IDay.GetPrevious(int numberOfDays)
        => this.GetPrevious(numberOfDays).Map(day => (IDay)day);

    #endregion Next and Previous
}
