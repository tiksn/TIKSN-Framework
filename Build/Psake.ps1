
Properties {
}

Task Default -Depends Publish

Task Publish -Depends Pack {
    $packageName = Get-ChildItem -Name TIKSN-Framework.*.nupkg
    
    nuget push "$packageName" -source nuget.org
}

Task Pack -Depends Build {
    Remove-Item TIKSN-Framework.*.nupkg
    
    nuget pack .\TIKSN-Framework.nuspec
}

Task Build -Depends Clean, TextTransform {
    & "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" '..\TIKSN Framework.sln' /t:Rebuild /p:Configuration=Release /v:m
}

Task TextTransform {
    & "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\TextTransform.exe" ..\TIKSN.Core\Localization\LocalizationKeys.tt
}

Task Clean {
    & "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" '..\TIKSN Framework.sln' /t:Clean /p:Configuration=Release /v:m
}