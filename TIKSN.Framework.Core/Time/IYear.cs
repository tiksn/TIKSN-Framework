using NodaTime;

namespace TIKSN.Time;

public interface IYear<TSelf>
    : IEquatable<TSelf>
    , IComparable<TSelf>
    , IComparable
    , ISpanFormattable
    , IYear
    where TSelf : IYear<TSelf>
{
    #region Next and Previous

    public new TSelf GetNext();

    public new TSelf GetNext(int numberOfYears);

    public new TSelf GetPrevious();

    public new TSelf GetPrevious(int numberOfYears);

    #endregion Next and Previous
}

public interface IYear
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

    public IYear GetNext();

    public IYear GetNext(int numberOfYears);

    public IYear GetPrevious();

    public IYear GetPrevious(int numberOfYears);

    #endregion Next and Previous

    #region Conversion

    public DateInterval ToDateInterval();

    #endregion Conversion
}
