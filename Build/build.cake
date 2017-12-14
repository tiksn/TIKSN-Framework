var target = Argument("target", "Default");

Task("Default")
  .IsDependentOn("Build")
  .Does(() =>
{
  Information("Hello World!");
});

Task("Build")
  .Does(() =>
{
  MSBuild("../TIKSN Framework.sln", configurator =>
    configurator.SetConfiguration("Release")
        .SetVerbosity(Verbosity.Minimal)
        .UseToolVersion(MSBuildToolVersion.VS2017));
});
RunTarget(target);