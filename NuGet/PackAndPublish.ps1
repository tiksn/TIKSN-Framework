Remove-Item TIKSN-Framework.*.nupkg

nuget pack .\TIKSN-Framework.nuspec

$packageName = Get-ChildItem -Name TIKSN-Framework.*.nupkg

nuget push "$packageName" -source nuget.org
