$scriptDuration = [System.Diagnostics.Stopwatch]::StartNew()
$buildDuration = [System.Diagnostics.Stopwatch]::StartNew()

& "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" '..\TIKSN Framework.sln' /t:Rebuild /p:Configuration=Release /v:m

$buildDuration.Stop()

.\PackAndPublish.ps1

$scriptDuration.Stop()

Write-Host "Build duration" $buildDuration.Elapsed.ToString()
Write-Host "Build and Publish script duration" $scriptDuration.Elapsed.ToString()
