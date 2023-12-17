using LanguageExt;

namespace TIKSN.Tests.Licensing;

public record TestEntitlements(
    string Name,
    int Quantity,
    Seq<byte> Salt,
    long CompanyId,
    long EmployeeId);
