#addin "Cake.Http"
#addin "Cake.Json"
#addin "Cake.ExtendedNuGet"
#addin nuget:?package=Cake.Twitter&version=0.6.0
#addin nuget:?package=Newtonsoft.Json&version=9.0.1
#addin nuget:?package=NuGet.Core&version=2.14.0
#tool "nuget:?package=Mono.TextTransform"
#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Tweet");
var solution = "../TIKSN Framework.sln";
var nuspec = "TIKSN-Framework.nuspec";
var nextVersionString = "";

using System.Linq;

Task("Tweet")
  .IsDependentOn("Publish")
  .Does(() =>
{
  var oAuthConsumerKey = EnvironmentVariable("TIKSN-Framework-ConsumerKey");
  var oAuthConsumerSecret = EnvironmentVariable("TIKSN-Framework-ConsumerSecret");
  var accessToken = EnvironmentVariable("TIKSN-Framework-AccessToken");
  var accessTokenSecret = EnvironmentVariable("TIKSN-Framework-AccessTokenSecret");

  TwitterSendTweet(oAuthConsumerKey, oAuthConsumerSecret, accessToken, accessTokenSecret, $"TIKSN Framework {nextVersionString} is published https://www.nuget.org/packages/TIKSN-Framework/{nextVersionString}");
});

Task("Publish")
  .Description("Publish NuGet package.")
  .IsDependentOn("Pack")
  .Does(() =>
{
 var package = string.Format("tools/TIKSN-Framework.{0}.nupkg", nextVersionString);

 NuGetPush(package, new NuGetPushSettings {
     Source = "nuget.org",
     ApiKey = EnvironmentVariable("TIKSN-Framework-ApiKey")
 });
});

Task("Pack")
  .Description("Pack NuGet package.")
  .IsDependentOn("Build")
  .IsDependentOn("EstimateNextVersion")
  //.IsDependentOn("Test")
  .Does(() =>
{
  var nuGetPackSettings = new NuGetPackSettings {
    Version = nextVersionString,
    OutputDirectory = "tools"
    };

  NuGetPack(nuspec, nuGetPackSettings);
});

Task("Test")
  .IsDependentOn("Build")
  .Does(() =>
{
  XUnit2("../TIKSN.Framework.Tests/bin/Release/TIKSN.Framework.Tests.dll");
  XUnit2("../UnitTests/bin/Release/netstandard2.0/UnitTests.dll");
});

Task("Build")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("TextTransform")
  .Does(() =>
{
  MSBuild(solution, configurator =>
    configurator.SetConfiguration("Release")
        .SetVerbosity(Verbosity.Minimal)
        .UseToolVersion(MSBuildToolVersion.VS2017)
        .SetMSBuildPlatform(MSBuildPlatform.x64)
        .SetPlatformTarget(PlatformTarget.MSIL)
        //.WithTarget("Rebuild")
        );

  MSBuild(solution, configurator =>
    configurator.SetConfiguration("Release")
        .SetVerbosity(Verbosity.Minimal)
        .UseToolVersion(MSBuildToolVersion.VS2017)
        .SetMSBuildPlatform(MSBuildPlatform.x64)
        .SetPlatformTarget(PlatformTarget.x64)
        //.WithTarget("Rebuild")
        );

  MSBuild(solution, configurator =>
    configurator.SetConfiguration("Release")
        .SetVerbosity(Verbosity.Minimal)
        .UseToolVersion(MSBuildToolVersion.VS2017)
        .SetMSBuildPlatform(MSBuildPlatform.x64)
        .SetPlatformTarget(PlatformTarget.x86)
        //.WithTarget("Rebuild")
        );

  MSBuild(solution, configurator =>
    configurator.SetConfiguration("Release")
        .SetVerbosity(Verbosity.Minimal)
        .UseToolVersion(MSBuildToolVersion.VS2017)
        .SetMSBuildPlatform(MSBuildPlatform.x64)
        .SetPlatformTarget(PlatformTarget.ARM)
        //.WithTarget("Rebuild")
        );
});

Task("TextTransform")
  .Does(() =>
{
  // var transform = File("../TIKSN.Core/Localization/LocalizationKeys.tt");
  // TransformTemplate(transform);

  using(var process = StartAndReturnProcess("C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/Common7/IDE/TextTransform.exe", new ProcessSettings{ Arguments = "../TIKSN.Core/Localization/LocalizationKeys.tt" }))
  {
      process.WaitForExit();
      var exitCode = process.GetExitCode();
      if (exitCode != 0)
        throw new Exception(string.Format("Exit code is {0} which is not a zero.", exitCode));
  }
});


Task("EstimateNextVersion")
  .Description("Estimate next version.")
  .Does(() =>
{
  var packageList = NuGetList("TIKSN-Framework", new NuGetListSettings {
      AllVersions = false,
      Prerelease = true
      });
  var latestPackage = packageList.Single();
  var versionString = latestPackage.Version;
  var lastDotIndex = versionString.LastIndexOf('.');
  var prereleaseNumber = int.Parse(versionString.Substring(lastDotIndex + 1));
  var nextPrereleaseNumber = prereleaseNumber + 1;
  nextVersionString = versionString.Substring(0, lastDotIndex + 1) + nextPrereleaseNumber;
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
  CleanDirectories("../**/bin/**");
  CleanDirectories("../**/obj/**");
});

RunTarget(target);