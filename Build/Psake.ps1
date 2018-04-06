
Properties {
}

Task Default -Depends Publish

Task Publish -Depends Pack {
    $packageName = Get-ChildItem -Name TIKSN-Framework.*.nupkg
    
    nuget push "$packageName" -source nuget.org
}

Task Pack -Depends Build, EstimateNextVersion {
    Remove-Item TIKSN-Framework.*.nupkg
    
    nuget pack .\TIKSN-Framework.nuspec -Version $script:nextVersionString
}

Task Build -Depends Clean, Restore, TextTransform {
    & "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" '..\TIKSN Framework.sln' /t:Rebuild /p:Configuration=Release /v:m
}

Task TextTransform {
    & "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\TextTransform.exe" ..\TIKSN.Core\Localization\LocalizationKeys.tt
}

Task Clean {
    & "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" '..\TIKSN Framework.sln' /t:Clean /p:Configuration=Release /v:m
}

Task Restore {
    nuget restore '..\TIKSN Framework.sln'
}

Task EstimateNextVersion {
    $versionString = (Invoke-RestMethod -Uri 'https://api-v2v3search-0.nuget.org/query?q=TIKSN-Framework&prerelease=true&take=1&semVerLevel=2.0.0').data[0].Version

    $lastDotIndex = $versionString.LastIndexOf('.')
    $prereleaseNumber = [System.Int32]::Parse($versionString.Substring($lastDotIndex + 1))
    $nextPrereleaseNumber = $prereleaseNumber + 1
    $script:nextVersionString = $versionString.Substring(0, $lastDotIndex + 1) + $nextPrereleaseNumber
}