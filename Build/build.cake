#addin "Cake.Http"
#addin "Cake.Json"
#addin nuget:?package=Newtonsoft.Json&version=9.0.1
#tool "nuget:?package=Mono.TextTransform"

var target = Argument("target", "Pack");
var solution = "../TIKSN Framework.sln";
var nextVersionString = "";

Task("Pack")
  .Description("Pack NuGet package.")
  .IsDependentOn("Build")
  .IsDependentOn("EstimateNextVersion")
  .Does(() =>
{
  var nuGetPackSettings = new NuGetPackSettings {
    Version= nextVersionString
    };

  NuGetPack("TIKSN-Framework.nuspec", nuGetPackSettings);
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
        .WithTarget("Rebuild"));
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
  string responseBody = HttpGet("https://api-v2v3search-0.nuget.org/query?q=TIKSN-Framework&prerelease=true&take=1&semVerLevel=2.0.0");
  var responseObject = DeserializeJson<dynamic>(responseBody);
  
  var versionString = responseObject.data[0].version.ToString();
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
  MSBuild(solution, configurator =>
    configurator.SetConfiguration("Release")
        .SetVerbosity(Verbosity.Minimal)
        .UseToolVersion(MSBuildToolVersion.VS2017)
        .SetMSBuildPlatform(MSBuildPlatform.x64)
        .SetPlatformTarget(PlatformTarget.MSIL)
        .WithTarget("Clean"));

  CleanDirectories("../**/bin/**");
  CleanDirectories("../**/obj/**");
});

RunTarget(target);