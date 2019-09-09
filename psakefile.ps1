Properties {
    $PackageId = 'TIKSN-Framework'
    $msbuild = Resolve-MSBuild.ps1
}

Task Tweet -depends Publish

Task Publish -depends Pack {
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
        @{PackageGroups = @($packages.Standdard, $packages.Core, $packages.Legacy, $packages.UWP, $packages.Android); ProjectFile = '.\TIKSN.Core\TIKSN.Core.csproj' }
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
}

Task Test -depends Build

Task Build -depends BuildLanguageLocalization, BuildRegionLocalization, BuildCommonCore, BuildNetCore, BuildNetFramework, BuildAndroid, BuildUWP, CreateReferenceAssembliesForUWP {
}

Task BuildLanguageLocalization -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.LanguageLocalization/TIKSN.LanguageLocalization.csproj'

    Exec { dotnet build $project /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildRegionLocalization -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.RegionLocalization/TIKSN.RegionLocalization.csproj'

    Exec { dotnet build $project /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildCommonCore -depends DownloadCurrencyCodes, EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.Core/TIKSN.Core.csproj'

    Exec { dotnet build $project /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildNetCore -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.Framework.Core/TIKSN.Framework.Core.csproj'

    Exec { dotnet build $project /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildNetFramework -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.Framework.Full/TIKSN.Framework.Full.csproj'

    Exec { dotnet msbuild $project /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildAndroid -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.Framework.Android/TIKSN.Framework.Android.csproj'

    Exec { & $msbuild $project /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildUWP -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.Framework.UWP/TIKSN.Framework.UWP.csproj'


    Exec { & $msbuild $project /p:Configuration=Release /p:version=$Script:NextVersion /p:Platform=x64 /p:OutDir=$script:x64BuildArtifactsFolder }
    Exec { & $msbuild $project /p:Configuration=Release /p:version=$Script:NextVersion /p:Platform=x86 /p:OutDir=$script:x86BuildArtifactsFolder }
    Exec { & $msbuild $project /p:Configuration=Release /p:version=$Script:NextVersion /p:Platform=arm /p:OutDir=$script:armBuildArtifactsFolder }
}

Task CreateReferenceAssembliesForUWP -depends EstimateVersions, BuildUWP {
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
