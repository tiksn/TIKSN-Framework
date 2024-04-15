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

    new TSelf GetNext();

    new TSelf GetNext(int numberOfYears);

    new TSelf GetPrevious();

    new TSelf GetPrevious(int numberOfYears);

    #endregion Next and Previous
}

public interface IYear
{
    #region Contains

    bool Contains(LocalDate localDate);

    bool Contains(LocalDateTime localDateTime);

    bool Contains(ZonedDateTime zonedDateTime);

    bool Contains(OffsetDate offsetDate);

    bool Contains(OffsetDateTime offsetDateTime);

    bool Contains(YearMonth yearMonth);

    #endregion Contains

    #region Next and Previous

    IYear GetNext();

    IYear GetNext(int numberOfYears);

    IYear GetPrevious();

    IYear GetPrevious(int numberOfYears);

    #endregion Next and Previous

    #region Conversion

    DateInterval ToDateInterval();

    #endregion Conversion
}
