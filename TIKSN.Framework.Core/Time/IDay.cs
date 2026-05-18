using LanguageExt;
using NodaTime;
using TIKSN.Navigation;

namespace TIKSN.Time;

public interface IDay<TSelf> :
    IEquatable<TSelf>,
    IComparable<TSelf>,
    IComparable,
    ISpanFormattable,
    IDay
    where TSelf : IDay<TSelf>
{
    #region Next and Previous

    public new Option<TSelf> GetNext();

    public new Option<TSelf> GetNext(int numberOfDays);

    public new Option<TSelf> GetPrevious();

    public new Option<TSelf> GetPrevious(int numberOfDays);

    #endregion Next and Previous
}

public interface IDay : ISequentialNavigator<IDay>
{
    #region Contains

    public bool Contains(LocalDate localDate);

    public bool Contains(LocalDateTime localDateTime);

    public bool Contains(ZonedDateTime zonedDateTime);

    public bool Contains(OffsetDate offsetDate);

    public bool Contains(OffsetDateTime offsetDateTime);

    #endregion Contains

    #region Next and Previous

    public Option<IDay> GetNext(int numberOfDays);

    public Option<IDay> GetPrevious(int numberOfDays);

    #endregion Next and Previous

    #region Conversion

    public LocalDate ToLocalDate();

    public DateInterval ToDateInterval();

    #endregion Conversion
}
