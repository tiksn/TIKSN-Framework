$scriptDuration = [System.Diagnostics.Stopwatch]::StartNew()

Remove-Item TIKSN-Framework.*.nupkg

nuget pack .\TIKSN-Framework.nuspec

$packageName = Get-ChildItem -Name TIKSN-Framework.*.nupkg

nuget push "$packageName" -source nuget.org

$scriptDuration.Stop()

Write-Host "Pack and Publish script duration" $scriptDuration.Elapsed.ToString()