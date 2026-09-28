@{
    PackageId                  = 'TIKSN-Framework'
    Solution                   = 'TIKSN Framework.slnx'
    ExamplesSolution           = '.\examples\Examples.slnx'

    CurrencyCodesUrl1          = 'https://www.six-group.com/dam/download/financial-information/data-center/iso-currrency/lists/list-one.xml'
    CurrencyCodesOutFile1      = 'TIKSN.Framework.Core/Finance/Resources/TableA1.xml'
    CurrencyCodesUrl2          = 'https://www.six-group.com/dam/download/financial-information/data-center/iso-currrency/lists/list-three.xml'
    CurrencyCodesOutFile2      = 'TIKSN.Framework.Core/Finance/Resources/TableA3.xml'

    CleanupCodeProfile         = 'TIKSN Cleanup'
    NugetApiKeySecretName      = 'TIKSN-Framework-ApiKey'
    NuspecFile                 = '.\TIKSN-Framework.nuspec'
    DirectoryPackagesFile      = '.\Directory.Packages.props'

    Projects                   = @{
        LanguageLocalization = 'TIKSN.LanguageLocalization/TIKSN.LanguageLocalization.csproj'
        RegionLocalization   = 'TIKSN.RegionLocalization/TIKSN.RegionLocalization.csproj'
        Core                 = 'TIKSN.Framework.Core/TIKSN.Framework.Core.csproj'
        Maui                 = 'TIKSN.Framework.Maui/TIKSN.Framework.Maui.csproj'
        IntegrationTests     = './TIKSN.Framework.IntegrationTests/TIKSN.Framework.IntegrationTests.csproj'
        CoreTests            = './TIKSN.Framework.Core.Tests/TIKSN.Framework.Core.Tests.csproj'
    }

    MauiBuildFrameworks        = @{
        Ios         = 'net10.0-ios'
        MacCatalyst = 'net10.0-maccatalyst'
        Android     = 'net10.0-android'
        Windows     = 'net10.0-windows10.0.19041.0'
    }

    DependencyTargetFrameworks = @{
        Core        = 'net10.0'
        Android     = 'net10.0-android21.0'
        Ios         = 'net10.0-ios14.2'
        MacCatalyst = 'net10.0-maccatalyst14.0'
        Windows     = 'net10.0-windows10.0.19041.0'
    }
}
