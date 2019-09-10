Properties {
    $PackageId = 'TIKSN-Framework'
    $msbuild = Resolve-MSBuild.ps1
}

Task Tweet -depends Publish {
    Set-TwitterOAuthSettings `
        -ApiKey [Environment]::GetEnvironmentVariable('TIKSN-Framework-ConsumerKey') `
        -ApiSecret [Environment]::GetEnvironmentVariable('TIKSN-Framework-ConsumerSecret') `
        -AccessToken [Environment]::GetEnvironmentVariable('TIKSN-Framework-AccessToken') `
        -AccessTokenSecret [Environment]::GetEnvironmentVariable('TIKSN-Framework-AccessTokenSecret')

    Send-TwitterStatuses_Update "TIKSN Framework $Script:NextVersion is published https://www.nuget.org/packages/$PackageId/$Script:NextVersion"
}

Task Publish -depends Pack {
    $packageName = Join-Path -Path $script:trashFolder -ChildPath 'TIKSN-Framework.nupkg'
    $apiKey = [Environment]::GetEnvironmentVariable('TIKSN-Framework-ApiKey')

    Exec { nuget push $packageName -ApiKey $apiKey -Source https://api.nuget.org/v3/index.json }
}

Task Pack -depends Build, Test {
    $temporaryNuspec = Join-Path -Path $script:trashFolder -ChildPath '.\TIKSN-Framework.nuspec'
    Copy-Item -Path '.\TIKSN-Framework.nuspec' -Destination $temporaryNuspec

    $packages = @{
        Standdard = New-Object System.Collections.Specialized.OrderedDictionary
        Core      = New-Object System.Collections.Specialized.OrderedDictionary
        Legacy    = New-Object System.Collections.Specialized.OrderedDictionary
        UWP       = New-Object System.Collections.Specialized.OrderedDictionary
        Android   = New-Object System.Collections.Specialized.OrderedDictionary
    }

    $projectMap = @(
        @{PackageGroups = @($packages.Standdard, $packages.Core, $packages.Legacy, $packages.UWP, $packages.Android); ProjectFile = '.\TIKSN.Core\TIKSN.Core.csproj' },
        @{PackageGroups = @($packages.Core); ProjectFile = '.\TIKSN.Framework.Core\TIKSN.Framework.Core.csproj' }
    )

    foreach ($projectMapEntry in $projectMap) {
        $project = [xml](Get-Content -Path $projectMapEntry.ProjectFile -Raw)

        foreach ($packageReference in $project.SelectNodes('//PackageReference')) {
            $packageId = $packageReference.Include
            $packageVersion = $packageReference.Version

            if ($null -ne $packageVersion) {
                foreach ($packageGroup in $projectMapEntry.PackageGroups) {
                    if ($packageGroup.Contains($packageId)) {
                        $existingVersion = $packageGroup[$packageId]
                        if ($existingVersion -ne $packageVersion) {
                            throw "There was a package mismatch. ($existingVersion, $packageVersion)"
                        }
                    }
                    else {
                        $packageGroup[$packageId] = $packageVersion
                    }

                }
            }
        }
    }

    $dependencyGroups = @(
        @{Packages = $packages.Standdard; TargetFramework = 'netstandard2.0' },
        @{Packages = $packages.Core; TargetFramework = 'netcoreapp2.2' },
        @{Packages = $packages.Legacy; TargetFramework = 'net48' },
        @{Packages = $packages.UWP; TargetFramework = 'uap10.0.17134' },
        @{Packages = $packages.Android; TargetFramework = 'MonoAndroid8.1' }
    )

    $nuspec = [xml](Get-Content -Path $temporaryNuspec -Raw)

    foreach ($dependencyGroup in $dependencyGroups) {
        $group = $nuspec.CreateElement("group", $nuspec.DocumentElement.NamespaceURI)
        $group.SetAttribute('targetFramework', $dependencyGroup.TargetFramework)

        foreach ($key in $dependencyGroup.Packages.Keys) {
            $dependency = $nuspec.CreateElement("dependency", $nuspec.DocumentElement.NamespaceURI)
            $dependency.SetAttribute('id', $key)
            $dependency.SetAttribute('version', $dependencyGroup.Packages[$key])
            $dependency.SetAttribute('exclude', 'Build,Analyzers')
            $group.AppendChild($dependency) | Out-Null
        }

        $nuspec.package.metadata.dependencies.AppendChild($group) | Out-Null
    }

    $nuspec.Save($temporaryNuspec)

    Exec { nuget pack $temporaryNuspec -Version $Script:NextVersion -BasePath $script:buildArtifactsFolder -OutputDirectory $script:trashFolder -OutputFileNamesWithoutVersion }
}

Task Test -depends Build {
    Exec { dotnet test '.\TIKSN.Framework.Core.Tests\TIKSN.Framework.Core.Tests.csproj' }
}

Task Build -depends BuildLanguageLocalization, BuildRegionLocalization, BuildCommonCore, BuildNetCore, BuildNetFramework, BuildAndroid, BuildUWP, CreateReferenceAssembliesForUWP {
}

Task BuildLanguageLocalization -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.LanguageLocalization/TIKSN.LanguageLocalization.csproj'

    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildRegionLocalization -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.RegionLocalization/TIKSN.RegionLocalization.csproj'

    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildCommonCore -depends DownloadCurrencyCodes, EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.Core/TIKSN.Core.csproj'

    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildNetCore -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.Framework.Core/TIKSN.Framework.Core.csproj'

    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildNetFramework -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.Framework.Full/TIKSN.Framework.Full.csproj'

    Exec { dotnet msbuild $project /v:m /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildAndroid -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.Framework.Android/TIKSN.Framework.Android.csproj'

    Exec { & $msbuild $project /v:m /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildUWP -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.Framework.UWP/TIKSN.Framework.UWP.csproj'


    Exec { & $msbuild $project /v:m /p:Configuration=Release /p:version=$Script:NextVersion /p:Platform=x64 /p:OutDir=$script:x64BuildArtifactsFolder }
    Exec { & $msbuild $project /v:m /p:Configuration=Release /p:version=$Script:NextVersion /p:Platform=x86 /p:OutDir=$script:x86BuildArtifactsFolder }
    Exec { & $msbuild $project /v:m /p:Configuration=Release /p:version=$Script:NextVersion /p:Platform=arm /p:OutDir=$script:armBuildArtifactsFolder }
}

Task CreateReferenceAssembliesForUWP -depends EstimateVersions, BuildUWP {
    $sourceFilePath = Join-Path -Path $script:x86BuildArtifactsFolder -ChildPath "TIKSN.Framework.UWP\TIKSN.Framework.UWP.dll"
    $destinationFilePath = Join-Path -Path $script:anyBuildArtifactsFolder -ChildPath "TIKSN.Framework.UWP.dll"

    Copy-Item -Path $sourceFilePath -Destination $destinationFilePath
    #TODO: Patch DLL

    $sourceFilePath = Join-Path -Path $script:x86BuildArtifactsFolder -ChildPath "TIKSN.Framework.UWP\TIKSN.Framework.UWP.xml"
    $destinationFilePath = Join-Path -Path $script:anyBuildArtifactsFolder -ChildPath "TIKSN.Framework.UWP.xml"

    Copy-Item -Path $sourceFilePath -Destination $destinationFilePath
}

Task EstimateVersions -depends Restore {
    if ($Version) {
        $Script:NextVersion = $Version
    }
    else {
        $foundPackages = Find-Package -Name $PackageId -AllVersions -AllowPrereleaseVersions -ProviderName NuGet -Source nuget.org

        $foundPackages = $foundPackages | Where-Object { $_.Name -eq $PackageId }

        $foundPackageVersions = $foundPackages | Select-Object -ExpandProperty Version
        $foundPackageVersions = $foundPackageVersions | Where-Object { [System.Management.Automation.SemanticVersion]::TryParse($_, [ref][System.Management.Automation.SemanticVersion]$value) }
        $foundPackageVersions = $foundPackageVersions | ForEach-Object { [System.Management.Automation.SemanticVersion]$_ }
        $foundPackageVersions = $foundPackageVersions | Sort-Object -Descending
        $latestPackageVersion = $foundPackageVersions | Select-Object -First 1

        $nextBuildLabel = ([int]$latestPackageVersion.BuildLabel) + 1

        $Script:NextVersion = [System.Management.Automation.SemanticVersion]::New($latestPackageVersion.Major, $latestPackageVersion.Minor, $latestPackageVersion.Patch, $latestPackageVersion.PreReleaseLabel, $nextBuildLabel)
    }
}

Task DownloadCurrencyCodes -depends Clean {
    Invoke-WebRequest -Uri 'https://www.currency-iso.org/dam/downloads/lists/list_one.xml' -OutFile 'TIKSN.Core/Finance/Resources/TableA1.xml'
    Invoke-WebRequest -Uri 'https://www.currency-iso.org/dam/downloads/lists/list_three.xml' -OutFile 'TIKSN.Core/Finance/Resources/TableA3.xml'
}

Task Restore -depends Clean {
    $solution = Resolve-Path -Path 'TIKSN Framework.sln'
    Exec { dotnet restore $solution }
}

Task Clean -depends Init {
}

Task Init {
    $date = Get-Date
    $ticks = $date.Ticks
    $trashFolder = Join-Path -Path . -ChildPath ".trash"
    $script:trashFolder = Join-Path -Path $trashFolder -ChildPath $ticks.ToString("D19")
    New-Item -Path $script:trashFolder -ItemType Directory | Out-Null
    $script:trashFolder = Resolve-Path -Path $script:trashFolder

    $script:buildArtifactsFolder = Join-Path -Path $script:trashFolder -ChildPath "artifacts"
    New-Item -Path $script:buildArtifactsFolder -ItemType Directory | Out-Null

    $script:anyBuildArtifactsFolder = Join-Path -Path $script:buildArtifactsFolder -ChildPath "any"
    New-Item -Path $script:anyBuildArtifactsFolder -ItemType Directory | Out-Null

    $script:armBuildArtifactsFolder = Join-Path -Path $script:buildArtifactsFolder -ChildPath "arm"
    New-Item -Path $script:armBuildArtifactsFolder -ItemType Directory | Out-Null

    $script:x64BuildArtifactsFolder = Join-Path -Path $script:buildArtifactsFolder -ChildPath "x64"
    New-Item -Path $script:x64BuildArtifactsFolder -ItemType Directory | Out-Null

    $script:x86BuildArtifactsFolder = Join-Path -Path $script:buildArtifactsFolder -ChildPath "x86"
    New-Item -Path $script:x86BuildArtifactsFolder -ItemType Directory | Out-Null
}
