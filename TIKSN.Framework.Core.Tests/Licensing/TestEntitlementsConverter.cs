using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using LanguageExt;
using LanguageExt.Common;
using TIKSN.Licensing;

namespace TIKSN.Tests.Licensing;

public class TestEntitlementsConverter : IEntitlementsConverter<TestEntitlements, TestLicenseEntitlements>
{
    public Validation<Error, TestLicenseEntitlements> Convert(
        TestEntitlements entitlements)
    {
        var errors = new List<Error>();
        var result = new TestLicenseEntitlements();

        if (entitlements == null)
        {
            errors.Add(Error.New(1272355232, "Value must not be NULL"));
        }
        else
        {
            if (string.IsNullOrWhiteSpace(entitlements.Name))
            {
                errors.Add(Error.New(1989842507, "Name is missing"));
            }
            else
            {
                result.Name = entitlements.Name;
            }

            if (entitlements.Quantity <= 0)
            {
                errors.Add(Error.New(622153499, "Quantity is invalid"));
            }
            else
            {
                result.Quantity = entitlements.Quantity;
            }

            if (entitlements.CompanyId <= 0)
            {
                errors.Add(Error.New(715969796, "CompanyId is invalid"));
            }
            else
            {
                result.CompanyId = entitlements.CompanyId;
            }

            if (entitlements.EmployeeId <= 0)
            {
                errors.Add(Error.New(314894957, "EmployeeId is invalid"));
            }
            else
            {
                result.EmployeeId = entitlements.EmployeeId;
            }

            if (entitlements.Salt.Count == 0)
            {
                errors.Add(Error.New(139152695, "Salt is missing"));
            }
            else
            {
                result.Salt = ByteString.CopyFrom([.. entitlements.Salt]);
            }
        }

        if (errors.Count > 0)
        {
            return errors.ToSeq();
        }

        return result;
    }

    public Validation<Error, TestEntitlements> Convert(
        TestLicenseEntitlements entitlementsData)
    {
        var errors = new List<Error>();

        if (entitlementsData == null)
        {
            errors.Add(Error.New(716270800, "Value must not be NULL"));
        }
        else
        {
            if (string.IsNullOrWhiteSpace(entitlementsData.Name))
            {
                errors.Add(Error.New(1989842507, "Name is missing"));
            }

            if (entitlementsData.Quantity <= 0)
            {
                errors.Add(Error.New(622153499, "Quantity is invalid"));
            }

            if (entitlementsData.CompanyId <= 0)
            {
                errors.Add(Error.New(715969796, "CompanyId is invalid"));
            }

            if (entitlementsData.EmployeeId <= 0)
            {
                errors.Add(Error.New(314894957, "EmployeeId is invalid"));
            }

            if (entitlementsData.Salt.Length == 0)
            {
                errors.Add(Error.New(139152695, "Salt is missing"));
            }
        }

        if (errors.Count > 0)
        {
            return errors.ToSeq();
        }

        return new TestEntitlements(
            entitlementsData.Name,
            entitlementsData.Quantity,
            entitlementsData.Salt.ToSeq(),
            entitlementsData.CompanyId,
            entitlementsData.EmployeeId);
    }
}
