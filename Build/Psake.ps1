
Properties {
}

Task Default -Depends Publish

Task Publish -Depends Pack {
    $packageName = Get-ChildItem -Name TIKSN-Framework.*.nupkg
    
    nuget push "$packageName" -source nuget.org
}

Task Pack -Depends Clean, Build {
    Remove-Item TIKSN-Framework.*.nupkg
    
    nuget pack .\TIKSN-Framework.nuspec
}

Task Build -Depends Clean {
    & "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" '..\TIKSN Framework.sln' /t:Rebuild /p:Configuration=Release /v:m
}

Task Clean {
    & "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" '..\TIKSN Framework.sln' /t:Clean /p:Configuration=Release /v:m
}