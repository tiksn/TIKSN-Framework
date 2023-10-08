using LanguageExt;

namespace TIKSN.Licensing.Tests;

public record TestEntitlements(
    string Name,
    int Quantity,
    Seq<byte> Salt,
    long CompanyId,
    long EmployeeId);
