using System.Net.Mail;
using Bond;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

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

    internal static Validation<Error, ArraySegment<byte>> ConvertFromUlid(Ulid serialNumber)
    {
        if (InvalidSerialNumbers.Contains(serialNumber))
        {
            return Error.New(877535586, "Invalid serial number");
        }

        return new ArraySegment<byte>(serialNumber.ToByteArray());
    }

    internal static Validation<Error, Ulid> ConvertToUlid(ArraySegment<byte> serialNumberBytes)
    {
        var serialNumber = new Ulid(serialNumberBytes);

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

    internal static Validation<Error, IBonded<LicenseParty>> ConvertFromParty(Party party) => party switch
    {
        IndividualParty individualParty => ConvertFromIndividual(individualParty).Map<IBonded<LicenseParty>>(x => new Bonded<LicenseIndividualParty>(x)),
        OrganizationParty organizationParty => ConvertFromOrganization(organizationParty).Map<IBonded<LicenseParty>>(x => new Bonded<LicenseOrganizationParty>(x)),
        _ => Error.New(2143629281, "Invalid party type"),
    };

    internal static Validation<Error, Party> ConvertToParty(IBonded<LicenseParty> licenseParty)
    {
        var licensePartyBase = licenseParty.Deserialize();
        return licensePartyBase.Kind switch
        {
            LicensePartyKind.Unknown => Error.New(2084794372, "Unknown party type"),
            LicensePartyKind.Individual => ConvertToIndividual(licenseParty.Deserialize<LicenseIndividualParty>()).Map<Party>(x => x),
            LicensePartyKind.Organization => ConvertToOrganization(licenseParty.Deserialize<LicenseOrganizationParty>()).Map<Party>(x => x),
            _ => Error.New(103216125, "Invalid party type"),
        };
    }

    private static Validation<Error, LicenseIndividualParty> ConvertFromIndividual(
        IndividualParty individualParty)
    {
        ArgumentNullException.ThrowIfNull(individualParty);

        var errors = new List<Error>();
        var result = new LicenseIndividualParty
        {
            Kind = LicensePartyKind.Individual,
        };

        if (string.IsNullOrWhiteSpace(individualParty.FirstName))
        {
            errors.Add(Error.New(679209978, "First Name is missing"));
        }
        else
        {
            result.FirstName = individualParty.FirstName;
        }

        if (string.IsNullOrWhiteSpace(individualParty.LastName))
        {
            errors.Add(Error.New(1769457724, "Last Name is missing"));
        }
        else
        {
            result.LastName = individualParty.LastName;
        }

        if (string.IsNullOrWhiteSpace(individualParty.FullName))
        {
            result.FullName = $"{individualParty.FirstName} {individualParty.LastName}";
        }
        else
        {
            result.FullName = individualParty.FullName;
        }

        SetBasePartyProperties(individualParty, errors, result);

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return result;
    }

    private static Validation<Error, LicenseOrganizationParty> ConvertFromOrganization(
        OrganizationParty organizationParty)
    {
        ArgumentNullException.ThrowIfNull(organizationParty);

        var errors = new List<Error>();
        var result = new LicenseOrganizationParty
        {
            Kind = LicensePartyKind.Organization,
        };

        if (string.IsNullOrWhiteSpace(organizationParty.LongName))
        {
            errors.Add(Error.New(692537357, "Long Name is missing"));
        }
        else
        {
            result.LongName = organizationParty.LongName;
        }

        if (string.IsNullOrWhiteSpace(organizationParty.ShortName))
        {
            errors.Add(Error.New(2134180828, "Short Name is missing"));
        }
        else
        {
            result.ShortName = organizationParty.ShortName;
        }

        SetBasePartyProperties(organizationParty, errors, result);

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return result;
    }

    private static Validation<Error, IndividualParty> ConvertToIndividual(
        LicenseIndividualParty licenseIndividualParty)
    {
        ArgumentNullException.ThrowIfNull(licenseIndividualParty);

        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(licenseIndividualParty.FirstName))
        {
            errors.Add(Error.New(1348133387, "First Name is missing"));
        }

        if (string.IsNullOrWhiteSpace(licenseIndividualParty.LastName))
        {
            errors.Add(Error.New(946047972, "Last Name is missing"));
        }

        var fullName = string.IsNullOrWhiteSpace(licenseIndividualParty.FullName)
            ? $"{licenseIndividualParty.FirstName} {licenseIndividualParty.LastName}"
            : licenseIndividualParty.FullName;

        var basePartyProperties = GetBasePartyProperties(licenseIndividualParty);

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return basePartyProperties
            .Map(x => new IndividualParty(
            licenseIndividualParty.FirstName,
            licenseIndividualParty.LastName,
            fullName,
            x.email,
            x.website));
    }

    private static Validation<Error, OrganizationParty> ConvertToOrganization(
        LicenseOrganizationParty licenseOrganizationParty)
    {
        ArgumentNullException.ThrowIfNull(licenseOrganizationParty);

        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(licenseOrganizationParty.LongName))
        {
            errors.Add(Error.New(1625517012, "Long Name is missing"));
        }

        if (string.IsNullOrWhiteSpace(licenseOrganizationParty.ShortName))
        {
            errors.Add(Error.New(280436811, "Short Name is missing"));
        }

        var basePartyProperties = GetBasePartyProperties(licenseOrganizationParty);

        if (errors.Count != 0)
        {
            return errors.ToSeq();
        }

        return basePartyProperties
            .Map(x => new OrganizationParty(
            licenseOrganizationParty.LongName,
            licenseOrganizationParty.ShortName,
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
