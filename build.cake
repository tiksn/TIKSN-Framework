#addin "Cake.Http"
#addin "Cake.Json"
#addin "Cake.ExtendedNuGet"
#addin nuget:?package=Cake.Twitter&version=0.6.0
#addin nuget:?package=Newtonsoft.Json&version=9.0.1
#addin nuget:?package=NuGet.Core&version=2.14.0
#addin nuget:?package=NuGet.Versioning&version=4.6.2
#addin "nuget:?package=Cake.Wyam"
#addin nuget:?package=Cake.Git
#addin nuget:?package=TIKSN-Cake&loaddependencies=true
#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=Wyam"

var target = Argument("target", "Tweet");
var configuration = Argument("configuration", "Release");
var solution = "TIKSN Framework.sln";
var nuspec = "TIKSN-Framework.nuspec";
var nuGetPackageId = "TIKSN-Framework";
var nextVersionString = "";

using System;
using System.Linq;
using NuGet.Versioning;

DirectoryPath buildArtifactsDir;
DirectoryPath anyBuildArtifactsDir;
DirectoryPath armBuildArtifactsDir;
DirectoryPath x64BuildArtifactsDir;
DirectoryPath x86BuildArtifactsDir;

Setup(context =>
{
    SetTrashParentDirectory(GitFindRootFromPath("."));
});

Teardown(context =>
{
});

Task("Tweet")
  .IsDependentOn("Publish")
  .Does(() =>
{
  var oAuthConsumerKey = EnvironmentVariable("TIKSN-Framework-ConsumerKey");
  var oAuthConsumerSecret = EnvironmentVariable("TIKSN-Framework-ConsumerSecret");
  var accessToken = EnvironmentVariable("TIKSN-Framework-AccessToken");
  var accessTokenSecret = EnvironmentVariable("TIKSN-Framework-AccessTokenSecret");

  TwitterSendTweet(oAuthConsumerKey, oAuthConsumerSecret, accessToken, accessTokenSecret, $"TIKSN Framework {nextVersionString} is published https://www.nuget.org/packages/{nuGetPackageId}/{nextVersionString}");
});

Task("BuildDocs")
  .IsDependentOn("Build")
  .Does(() =>
{
    Wyam(new WyamSettings {
      OutputPath = Directory("./docs/")
    });
});
    
Task("PreviewDocs")
  .IsDependentOn("BuildDocs")
  .Does(() =>
{
    Wyam(new WyamSettings
    {
        Preview = true,
        Watch = true
    });        
});

Task("Publish")
  .Description("Publish NuGet package.")
  .IsDependentOn("Pack")
  .Does(() =>
{
  var package = string.Format("{0}/{1}.{2}.nupkg", GetTrashDirectory(), nuGetPackageId, nextVersionString);

  NuGetPush(package, new NuGetPushSettings {
     Source = "nuget.org",
     ApiKey = EnvironmentVariable("TIKSN-Framework-ApiKey")
  });
});

Task("Pack")
  .Description("Pack NuGet package.")
  .IsDependentOn("Build")
  .IsDependentOn("EstimateNextVersion")
  .IsDependentOn("Test")
  .IsDependentOn("BuildDocs")
  .Does(() =>
{
  var nuGetPackSettings = new NuGetPackSettings {
    Version = nextVersionString,
    OutputDirectory = GetTrashDirectory()
    };

  NuGetPack(nuspec, nuGetPackSettings);
});

Task("Test")
  .IsDependentOn("Build")
  .Does(() =>
{
  XUnit2(new [] {
    anyBuildArtifactsDir.CombineWithFilePath("TIKSN.Framework.Full.Tests.dll")
     },
     new XUnit2Settings {
        Parallelism = ParallelismOption.All,
        HtmlReport = true,
        XmlReport = true,
        NoAppDomain = true,
        OutputDirectory = CreateTrashSubDirectory("test-results")
    });
});

Task("Build")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("GenerateLocalizationKeys")
  .IsDependentOn("DownloadCurrencyCodes")
  .Does(() =>
{
  buildArtifactsDir = CreateTrashSubDirectory("artifacts");

  anyBuildArtifactsDir = buildArtifactsDir.Combine("any");
  EnsureDirectoryExists(anyBuildArtifactsDir);

  armBuildArtifactsDir = buildArtifactsDir.Combine("arm");
  EnsureDirectoryExists(armBuildArtifactsDir);

  x64BuildArtifactsDir = buildArtifactsDir.Combine("x64");
  EnsureDirectoryExists(x64BuildArtifactsDir);

  x86BuildArtifactsDir = buildArtifactsDir.Combine("x86");
  EnsureDirectoryExists(x86BuildArtifactsDir);

  MSBuild(solution, configurator =>
    configurator.SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal)
        .UseToolVersion(MSBuildToolVersion.VS2017)
        .SetMSBuildPlatform(MSBuildPlatform.x64)
        .SetPlatformTarget(PlatformTarget.MSIL)
        .WithProperty("OutDir", anyBuildArtifactsDir.FullPath)
        //.WithTarget("Rebuild")
        );

  MSBuild(solution, configurator =>
    configurator.SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal)
        .UseToolVersion(MSBuildToolVersion.VS2017)
        .SetMSBuildPlatform(MSBuildPlatform.x64)
        .SetPlatformTarget(PlatformTarget.x64)
        .WithProperty("OutDir", x64BuildArtifactsDir.FullPath)
        //.WithTarget("Rebuild")
        );

  MSBuild(solution, configurator =>
    configurator.SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal)
        .UseToolVersion(MSBuildToolVersion.VS2017)
        .SetMSBuildPlatform(MSBuildPlatform.x64)
        .SetPlatformTarget(PlatformTarget.x86)
        .WithProperty("OutDir", x86BuildArtifactsDir.FullPath)
        //.WithTarget("Rebuild")
        );

  MSBuild(solution, configurator =>
    configurator.SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal)
        .UseToolVersion(MSBuildToolVersion.VS2017)
        .SetMSBuildPlatform(MSBuildPlatform.x64)
        .SetPlatformTarget(PlatformTarget.ARM)
        .WithProperty("OutDir", armBuildArtifactsDir.FullPath)
        //.WithTarget("Rebuild")
        );
});

Task("DownloadCurrencyCodes")
  .Does(() =>
{
  DownloadFile("https://www.currency-iso.org/dam/downloads/lists/list_one.xml", File("TIKSN.Core/Finance/Resources/TableA1.xml"));
  DownloadFile("https://www.currency-iso.org/dam/downloads/lists/list_three.xml", File("TIKSN.Core/Finance/Resources/TableA3.xml"));
});

Task("GenerateLocalizationKeys")
  .Does(() =>
{
  GenerateLocalizationKeys(
    "TIKSN.Localization",
    "LocalizationKeys",
    Directory("TIKSN.Core/Localization"),
    File("TIKSN.Core/Localization/EnglishOnly.resx"),
    File("TIKSN.Core/Shell/ShellCommand.resx"),
    File("TIKSN.LanguageLocalization/Strings.resx"),
    File("TIKSN.RegionLocalization/Strings.resx"));
});


Task("EstimateNextVersion")
  .Description("Estimate next version.")
  .Does(() =>
{
  var packageList = NuGetList(nuGetPackageId, new NuGetListSettings {
      AllVersions = false,
      Prerelease = true
      });
  var latestPackage = packageList.Single();
  var latestPackageNuGetVersion = new NuGetVersion(latestPackage.Version);

  if(!latestPackageNuGetVersion.IsPrerelease)
    throw new FormatException("Latest package version is not pre-release version.");

  if(latestPackageNuGetVersion.ReleaseLabels.Count() != 2)
    throw new FormatException("Latest package version should have exactly 2 pre-release labels.");

  var prereleaseNumber = int.Parse(latestPackageNuGetVersion.ReleaseLabels.ElementAt(1));
  var nextPrereleaseNumber = prereleaseNumber + 1;

  var nextReleaseLabels = latestPackageNuGetVersion.ReleaseLabels.ToArray();
  nextReleaseLabels[1] = nextPrereleaseNumber.ToString();
  var nextVersion = new NuGetVersion(latestPackageNuGetVersion.Version, nextReleaseLabels, null, null);
  nextVersionString = nextVersion.ToString();
  Information("Next version estimated to be " + nextVersionString);
});

Task("Restore")
  .Description("Restores packages.")
  .Does(() =>
{
  NuGetRestore(solution);
});

Task("Clean")
  .Description("Cleans all directories that are used during the build process.")
  .Does(() =>
{
  CleanDirectories("**/bin/**");
  CleanDirectories("**/obj/**");
});

RunTarget(target);