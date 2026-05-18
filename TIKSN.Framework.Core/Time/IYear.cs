using LanguageExt;
using NodaTime;
using TIKSN.Navigation;

namespace TIKSN.Time;

public interface IYear<TSelf> :
    IEquatable<TSelf>,
    IComparable<TSelf>,
    IComparable,
    ISpanFormattable,
    IYear
    where TSelf : IYear<TSelf>
{
    #region Next and Previous

    public new Option<TSelf> GetNext();

    public new Option<TSelf> GetNext(int numberOfYears);

    public new Option<TSelf> GetPrevious();

    public new Option<TSelf> GetPrevious(int numberOfYears);

    #endregion Next and Previous
}

public interface IYear : ISequentialNavigator<IYear>
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

    public Option<IYear> GetNext(int numberOfYears);

    public Option<IYear> GetPrevious(int numberOfYears);

    #endregion Next and Previous

    #region Conversion

    public DateInterval ToDateInterval();

    #endregion Conversion
}
