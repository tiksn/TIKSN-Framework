using System.Net.Mail;
using Google.Protobuf;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using static TIKSN.Licensing.LicenseParty;

namespace TIKSN.Licensing;

internal static class LicenseFactoryTermsValidation
{
    #region Date

    private static readonly Seq<DateTimeOffset> InvalidDates =
        Seq(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);

    internal static Validation<Error, long> ConvertFromDate(DateTimeOffset instant)
    {
        if (InvalidDates.Contains(instant))
        {
            return Error.New(877535586, "Invalid date");
        }

        return instant.UtcTicks;
    }

    internal static Validation<Error, DateTimeOffset> ConvertToDate(long dateTicks)
    {
        if (InvalidDates.Map(x => x.Ticks).Contains(dateTicks))
        {
            return Error.New(1106123090, "Invalid date ticks");
        }

        return new DateTimeOffset(dateTicks, TimeSpan.Zero);
    }

    #endregion Date

    #region ULID

    private static readonly Seq<Ulid> InvalidSerialNumbers =
        Seq(Ulid.Empty, Ulid.MinValue, Ulid.MaxValue);

    internal static Validation<Error, ByteString> ConvertFromUlid(Ulid serialNumber)
    {
        if (InvalidSerialNumbers.Contains(serialNumber))
        {
            return Error.New(877535586, "Invalid serial number");
        }

        return ByteString.CopyFrom(serialNumber.ToByteArray());
    }

    internal static Validation<Error, Ulid> ConvertToUlid(ByteString serialNumberBytes)
    {
        var serialNumber = new Ulid(serialNumberBytes.ToByteArray());

        if (InvalidSerialNumbers.Contains(serialNumber))
        {
            return Error.New(223867871, "Invalid serial number");
        }

        return serialNumber;
    }

    #endregion ULID

    #region URI

    private static readonly Seq<string> ValidUriSchemes =
        Seq(Uri.UriSchemeHttp, Uri.UriSchemeHttps);

    internal static Validation<Error, string> ConvertFromUri(Uri address)
    {
        if (string.IsNullOrWhiteSpace(address?.AbsoluteUri))
        {
            return Error.New(594495435, "URL is missing");
        }

        if (!ValidUriSchemes.Contains(address.Scheme))
        {
            return Error.New(829727941, "Invalid URL scheme");
        }

        return address.AbsoluteUri;
    }

    internal static Validation<Error, Uri> ConvertToUri(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return Error.New(1063052190, "URL is missing");
        }

        var result = new Uri(address);

        if (!ValidUriSchemes.Contains(result.Scheme))
        {
            return Error.New(122011466, "Invalid URL scheme");
        }

        return result;
    }

    #endregion URI

    #region Mail Address

    internal static Validation<Error, string> ConvertFromMailAddress(MailAddress address)
    {
        if (string.IsNullOrWhiteSpace(address?.Address))
        {
            return Error.New(1048625068, "Mail Address is missing");
        }

        return address.Address;
    }

    internal static Validation<Error, MailAddress> ConvertToMailAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return Error.New(2037180511, "Mail Address is missing");
        }

        return new MailAddress(address);
    }

    #endregion Mail Address

    #region Party

    internal static Validation<Error, LicenseParty> ConvertFromParty(Party party) => party switch
    {
        IndividualParty individualParty => ConvertFromIndividual(individualParty),
        OrganizationParty organizationParty => ConvertFromOrganization(organizationParty),
        _ => Error.New(2143629281, "Invalid party type"),
    };

    internal static Validation<Error, Party> ConvertToParty(LicenseParty licenseParty) =>
        licenseParty.PartyKindCase switch
        {
            PartyKindOneofCase.None => Error.New(2084794372, "Unknown or No party type"),
            PartyKindOneofCase.IndividualParty => ConvertToIndividual(licenseParty).Map<Party>(x => x),
            PartyKindOneofCase.OrganizationParty => ConvertToOrganization(licenseParty).Map<Party>(x => x),
            _ => Error.New(103216125, "Invalid party type"),
        };

    private static Validation<Error, LicenseParty> ConvertFromIndividual(
        IndividualParty individualParty)
    {
        ArgumentNullException.ThrowIfNull(individualParty);

        var errors = new List<Error>();
        var result = new LicenseParty
        {
            IndividualParty = new LicenseIndividualParty()
        };

        if (string.IsNullOrWhiteSpace(individualParty.FirstName))
        {
            errors.Add(Error.New(679209978, "First Name is missing"));
        }
        else
        {
            result.IndividualParty.FirstName = individualParty.FirstName;
        }

        if (string.IsNullOrWhiteSpace(individualParty.LastName))
        {
            errors.Add(Error.New(1769457724, "Last Name is missing"));
        }
        else
        {
            result.IndividualParty.LastName = individualParty.LastName;
        }

        if (string.IsNullOrWhiteSpace(individualParty.FullName))
        {
            result.IndividualParty.FullName = $"{individualParty.FirstName} {individualParty.LastName}";
        }
        else
        {
            result.IndividualParty.FullName = individualParty.FullName;
        }

        SetBasePartyProperties(individualParty, errors, result);

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return result;
    }

    private static Validation<Error, LicenseParty> ConvertFromOrganization(
        OrganizationParty organizationParty)
    {
        ArgumentNullException.ThrowIfNull(organizationParty);

        var errors = new List<Error>();
        var result = new LicenseParty
        {
            OrganizationParty = new LicenseOrganizationParty()
        };

        if (string.IsNullOrWhiteSpace(organizationParty.LongName))
        {
            errors.Add(Error.New(692537357, "Long Name is missing"));
        }
        else
        {
            result.OrganizationParty.LongName = organizationParty.LongName;
        }

        if (string.IsNullOrWhiteSpace(organizationParty.ShortName))
        {
            errors.Add(Error.New(2134180828, "Short Name is missing"));
        }
        else
        {
            result.OrganizationParty.ShortName = organizationParty.ShortName;
        }

        SetBasePartyProperties(organizationParty, errors, result);

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return result;
    }

    private static Validation<Error, IndividualParty> ConvertToIndividual(
        LicenseParty licenseParty)
    {
        ArgumentNullException.ThrowIfNull(licenseParty);

        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(licenseParty.IndividualParty.FirstName))
        {
            errors.Add(Error.New(1348133387, "First Name is missing"));
        }

        if (string.IsNullOrWhiteSpace(licenseParty.IndividualParty.LastName))
        {
            errors.Add(Error.New(946047972, "Last Name is missing"));
        }

        var fullName = string.IsNullOrWhiteSpace(licenseParty.IndividualParty.FullName)
            ? $"{licenseParty.IndividualParty.FirstName} {licenseParty.IndividualParty.LastName}"
            : licenseParty.IndividualParty.FullName;

        var basePartyProperties = GetBasePartyProperties(licenseParty);

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return basePartyProperties
            .Map(x => new IndividualParty(
            licenseParty.IndividualParty.FirstName,
            licenseParty.IndividualParty.LastName,
            fullName,
            x.email,
            x.website));
    }

    private static Validation<Error, OrganizationParty> ConvertToOrganization(
        LicenseParty licenseParty)
    {
        ArgumentNullException.ThrowIfNull(licenseParty);

        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(licenseParty.OrganizationParty.LongName))
        {
            errors.Add(Error.New(1625517012, "Long Name is missing"));
        }

        if (string.IsNullOrWhiteSpace(licenseParty.OrganizationParty.ShortName))
        {
            errors.Add(Error.New(280436811, "Short Name is missing"));
        }

        var basePartyProperties = GetBasePartyProperties(licenseParty);

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return basePartyProperties
            .Map(x => new OrganizationParty(
            licenseParty.OrganizationParty.LongName,
            licenseParty.OrganizationParty.ShortName,
            x.email,
            x.website));
    }

    private static Validation<Error, (MailAddress email, Uri website)> GetBasePartyProperties(
        LicenseParty party)
    {
        var emailValidation = ConvertToMailAddress(party.Email);
        var websiteValidation = ConvertToUri(party.Website);

        return emailValidation.Bind(e => websiteValidation.Map(w => (e, w)));
    }

    private static void SetBasePartyProperties(
        Party party,
        List<Error> errors,
        LicenseParty result)
    {
        _ = ConvertFromMailAddress(party.Email)
            .Match(succ => result.Email = succ, fail => errors.AddRange(fail));

        _ = ConvertFromUri(party.Website)
            .Match(succ => result.Website = succ, fail => errors.AddRange(fail));
    }

    #endregion Party
}
