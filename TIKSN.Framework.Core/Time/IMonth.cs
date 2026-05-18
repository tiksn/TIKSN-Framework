using LanguageExt;
using NodaTime;
using TIKSN.Navigation;

namespace TIKSN.Time;

public interface IMonth<TSelf> :
    IEquatable<TSelf>,
    IComparable<TSelf>,
    IComparable,
    ISpanFormattable,
    IMonth
    where TSelf : IMonth<TSelf>
{
    #region Next and Previous

    public new Option<TSelf> GetNext();

    public new Option<TSelf> GetNext(int numberOfMonths);

    public new Option<TSelf> GetPrevious();

    public new Option<TSelf> GetPrevious(int numberOfMonths);

    #endregion Next and Previous
}

public interface IMonth : ISequentialNavigator<IMonth>
{
    #region Contains

    public bool Contains(LocalDate localDate);

    public bool Contains(LocalDateTime localDateTime);

    public bool Contains(ZonedDateTime zonedDateTime);

    public bool Contains(OffsetDate offsetDate);

    public bool Contains(OffsetDateTime offsetDateTime);

    public bool Contains(YearMonth yearMonth);

    #endregion Contains

    #region Next and Previous

    public Option<IMonth> GetNext(int numberOfMonths);

    public Option<IMonth> GetPrevious(int numberOfMonths);

    #endregion Next and Previous

    #region Conversion

    public DateInterval ToDateInterval();

    #endregion Conversion
}
