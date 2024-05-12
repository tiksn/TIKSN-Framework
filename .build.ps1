<#
.Synopsis
    Build script

.Description
    TASKS AND REQUIREMENTS
    Initialize and Clean repository
    Restore packages, workflows, tools
    Format code
    Build projects and the solution
    Run Tests
    Pack
    Publish
#>

param(
    # Build Version
    [Parameter()]
    [string]
    $Version,
    # Build Instance
    [Parameter()]
    [string]
    $Instance
)

Set-StrictMode -Version Latest

# Synopsis: Publish NuGet package
Task Publish Pack, ValidateVersion, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $trashFolder = $state.TrashFolder
    $packageName = Join-Path -Path $trashFolder -ChildPath 'TIKSN-Framework.nupkg'

    if ($null -eq $env:NUGET_API_KEY) {
        Import-Module -Name Microsoft.PowerShell.SecretManagement
        $apiKey = Get-Secret -Name 'TIKSN-Framework-ApiKey' -AsPlainText
    }
    else {
        $apiKey = $env:NUGET_API_KEY
    }

    Exec { nuget push $packageName -ApiKey $apiKey -Source https://api.nuget.org/v3/index.json }
}

# Synopsis: Validate Next Version
Task ValidateVersion EstimateVersion, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $nextVersion = [System.Management.Automation.SemanticVersion]::Parse($state.NextVersion.ToString())

    $gitTags = git tag
    $gitTagVersions = $gitTags | ForEach-Object { [System.Management.Automation.SemanticVersion]::Parse($_) }
    $gitTagVersions = $gitTagVersions | Sort-Object -Descending
    $latestTagVersion = $gitTagVersions | Select-Object -First 1

    if ($nextVersion -le $latestTagVersion) {
        throw "Next Release version '$nextVersion' should be greater than latest tag version '$latestTagVersion'"
    }
}

# Synopsis: Pack NuGet package
Task Pack Build, Test, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $trashFolder = $state.TrashFolder
    $buildArtifactsFolder = $state.BuildArtifactsFolder
    $temporaryNuspec = Join-Path -Path $trashFolder -ChildPath '.\TIKSN-Framework.nuspec'
    Copy-Item -Path '.\TIKSN-Framework.nuspec' -Destination $temporaryNuspec

    $packages = @{
        Core        = New-Object System.Collections.Specialized.OrderedDictionary
        Android     = New-Object System.Collections.Specialized.OrderedDictionary
        IOS         = New-Object System.Collections.Specialized.OrderedDictionary
        MacCatalyst = New-Object System.Collections.Specialized.OrderedDictionary
        Windows     = New-Object System.Collections.Specialized.OrderedDictionary
    }

    $projectMap = @(
        @{PackageGroups = @($packages.Core, $packages.Android, $packages.IOS, $packages.MacCatalyst, $packages.Windows); ProjectFile = '.\TIKSN.Framework.Core\TIKSN.Framework.Core.csproj' }
        @{PackageGroups = @($packages.Android); ProjectFile = '.\TIKSN.Framework.Maui\TIKSN.Framework.Maui.csproj' }
        @{PackageGroups = @($packages.IOS); ProjectFile = '.\TIKSN.Framework.Maui\TIKSN.Framework.Maui.csproj' }
        @{PackageGroups = @($packages.MacCatalyst); ProjectFile = '.\TIKSN.Framework.Maui\TIKSN.Framework.Maui.csproj' }
        @{PackageGroups = @($packages.Windows); ProjectFile = '.\TIKSN.Framework.Maui\TIKSN.Framework.Maui.csproj' }
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
                            throw "There was a package $packageId mismatch. ($existingVersion, $packageVersion)"
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
        @{Packages = $packages.Core; TargetFramework = 'net8.0' },
        @{Packages = $packages.Android; TargetFramework = 'net8.0-android21.0' }
        @{Packages = $packages.IOS; TargetFramework = 'net8.0-ios14.2' }
        @{Packages = $packages.MacCatalyst; TargetFramework = 'net8.0-maccatalyst14.0' }
        @{Packages = $packages.Windows; TargetFramework = 'net8.0-windows10.0.19041.0' }
    )

    $nuspec = [xml](Get-Content -Path $temporaryNuspec -Raw)

    foreach ($dependencyGroup in $dependencyGroups) {
        $group = $nuspec.CreateElement('group', $nuspec.DocumentElement.NamespaceURI)
        $group.SetAttribute('targetFramework', $dependencyGroup.TargetFramework)

        foreach ($key in $dependencyGroup.Packages.Keys) {
            $dependency = $nuspec.CreateElement('dependency', $nuspec.DocumentElement.NamespaceURI)
            $dependency.SetAttribute('id', $key)
            $dependency.SetAttribute('version', $dependencyGroup.Packages[$key])
            $dependency.SetAttribute('exclude', 'Build,Analyzers')
            $group.AppendChild($dependency) | Out-Null
        }

        $nuspec.package.metadata.dependencies.AppendChild($group) | Out-Null
    }

    $multilingualResourcesFolders = Get-ChildItem -Path 'MultilingualResources' -Recurse -Directory
    foreach ($multilingualResourcesFolder in $multilingualResourcesFolders) {

        $projectName = $multilingualResourcesFolder.Parent.Name
        $projectComment = $nuspec.CreateComment($projectName)
        $nuspec.package.files.AppendChild($projectComment) | Out-Null

        foreach ($dependencyGroup in $dependencyGroups) {

            foreach ($mainFileExtension in ('dll', 'xml', 'pdb')) {
                $file = $nuspec.CreateElement('file', $nuspec.DocumentElement.NamespaceURI)
                $file.SetAttribute('src', "any\$projectName.$mainFileExtension")
                $file.SetAttribute('target', "lib\$($dependencyGroup.TargetFramework)")
                $nuspec.package.files.AppendChild($file) | Out-Null
            }

            $multilingualResourcesFiles = Get-ChildItem -Path $multilingualResourcesFolder
            foreach ($multilingualResourcesFile in $multilingualResourcesFiles) {
                $nameParts = $multilingualResourcesFile.Name -split '\.'
                $code = $nameParts[-2]

                $file = $nuspec.CreateElement('file', $nuspec.DocumentElement.NamespaceURI)
                $file.SetAttribute('src', "any\$code\$projectName.resources.dll")
                $file.SetAttribute('target', "lib\$($dependencyGroup.TargetFramework)\$code")
                $nuspec.package.files.AppendChild($file) | Out-Null
            }
        }
    }

    $nuspec.Save($temporaryNuspec)

    Copy-Item -Path 'icon.png' -Destination $buildArtifactsFolder
    Copy-Item -Path 'README.md' -Destination $buildArtifactsFolder
    Exec { nuget pack $temporaryNuspec -Version $state.NextVersion -BasePath $buildArtifactsFolder -OutputDirectory $trashFolder -OutputFileNamesWithoutVersion -Verbosity detailed }
}

# Synopsis: Test
Task Test Build, {
    Exec { dotnet test '.\TIKSN.Framework.Core.Tests\TIKSN.Framework.Core.Tests.csproj' }

    if (-not $env:CI) {
        Exec { dotnet test '.\TIKSN.Framework.IntegrationTests\TIKSN.Framework.IntegrationTests.csproj' }
    }
}

# Synopsis: Build
Task Build Format, BuildLanguageLocalization, BuildRegionLocalization, BuildCore, BuildMaui, {
    $solution = Resolve-Path -Path 'TIKSN Framework.sln'
    Exec { dotnet build $solution }
}

# Synopsis: Build Language Localization
Task BuildLanguageLocalization EstimateVersion, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $anyBuildArtifactsFolder = $state.AnyBuildArtifactsFolder
    $project = Resolve-Path -Path 'TIKSN.LanguageLocalization/TIKSN.LanguageLocalization.csproj'
    $nextVersion = $state.NextVersion

    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$anyBuildArtifactsFolder }
}

# Synopsis: Build Region Localization
Task BuildRegionLocalization EstimateVersion, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $anyBuildArtifactsFolder = $state.AnyBuildArtifactsFolder
    $project = Resolve-Path -Path 'TIKSN.RegionLocalization/TIKSN.RegionLocalization.csproj'
    $nextVersion = $state.NextVersion

    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$anyBuildArtifactsFolder }
}

# Synopsis: Build Core
Task BuildCore EstimateVersion, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $anyBuildArtifactsFolder = $state.AnyBuildArtifactsFolder
    $project = Resolve-Path -Path 'TIKSN.Framework.Core/TIKSN.Framework.Core.csproj'
    $nextVersion = $state.NextVersion

    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$anyBuildArtifactsFolder }
}

# Synopsis: Build MAUI
Task BuildMaui EstimateVersion, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $anyBuildArtifactsFolder = $state.AnyBuildArtifactsFolder
    $anyIosBuildArtifactsFolder = $state.AnyIosBuildArtifactsFolder
    $anyMaccatalystBuildArtifactsFolder = $state.AnyMaccatalystBuildArtifactsFolder
    $anyAndroidBuildArtifactsFolder = $state.AnyAndroidBuildArtifactsFolder
    $anyWindowsBuildArtifactsFolder = $state.AnyWindowsBuildArtifactsFolder
    $project = Resolve-Path -Path 'TIKSN.Framework.Maui/TIKSN.Framework.Maui.csproj'
    $nextVersion = $state.NextVersion

    Exec { dotnet build $project /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$anyBuildArtifactsFolder }

    Exec { dotnet build $project --framework net8.0-ios /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$anyIosBuildArtifactsFolder }
    Exec { dotnet build $project --framework net8.0-maccatalyst /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$anyMaccatalystBuildArtifactsFolder }
    Exec { dotnet build $project --framework net8.0-android /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$anyAndroidBuildArtifactsFolder }
    Exec { dotnet build $project --framework net8.0-windows10.0.19041.0 /v:m /p:Configuration=Release /p:version=$nextVersion /p:OutDir=$anyWindowsBuildArtifactsFolder }
}

# Synopsis: Estimate Next Version
Task EstimateVersion Restore, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    if ($Version) {
        $state.NextVersion = [System.Management.Automation.SemanticVersion]$Version
    }
    else {
        $currentCommit = git rev-parse HEAD
        [System.Management.Automation.SemanticVersion]$headTagVersion = git tag --points-at HEAD

        if ($null -eq $headTagVersion) {
            $foundPackages = Find-Package -Name $state.PackageId -AllVersions -AllowPrereleaseVersions -ProviderName NuGet -Source nuget.org

            $foundPackages = $foundPackages | Where-Object { $_.Name -eq $state.PackageId }

            $value = [System.Management.Automation.SemanticVersion]::New(0)
            $foundPackageVersions = $foundPackages | Select-Object -ExpandProperty Version
            $foundPackageVersions = $foundPackageVersions | Where-Object { [System.Management.Automation.SemanticVersion]::TryParse($_, [ref][System.Management.Automation.SemanticVersion]$value) }
            $foundPackageVersions = $foundPackageVersions | ForEach-Object { [System.Management.Automation.SemanticVersion]$_ }
            $foundPackageVersions = $foundPackageVersions | Sort-Object -Descending
            $latestPackageVersion = $foundPackageVersions | Select-Object -First 1

            if ($null -eq $latestPackageVersion.PreReleaseLabel) {
                $nextPreReleaseLabel = 'alpha.1'

                $state.NextVersion = [System.Management.Automation.SemanticVersion]::New($latestPackageVersion.Major, $latestPackageVersion.Minor, $latestPackageVersion.Patch + 1, $nextPreReleaseLabel, $currentCommit)
            }
            else {
                $nextPreReleaseLabel = $latestPackageVersion.PreReleaseLabel.Split('.')[0] + '.' + (([int]$latestPackageVersion.PreReleaseLabel.Split('.')[1]) + 1)

                $state.NextVersion = [System.Management.Automation.SemanticVersion]::New($latestPackageVersion.Major, $latestPackageVersion.Minor, $latestPackageVersion.Patch, $nextPreReleaseLabel, $currentCommit)
            }
        }
        else {
            $state.NextVersion = [System.Management.Automation.SemanticVersion]::New($headTagVersion.Major, $headTagVersion.Minor, $headTagVersion.Patch, $headTagVersion.PreReleaseLabel, $currentCommit)
        }
    }

    $state.NextVersion
    $state | Export-Clixml -Path ".\.trash\$Instance\state.clixml"
    Write-Output "Next version estimated to be $($state.NextVersion)"
    Write-Output $state
}

# Synopsis: Format
Task Format Restore, FormatWhitespace, FormatStyle, FormatAnalyzers

# Synopsis: Format Analyzers
Task FormatAnalyzers Restore, FormatAnalyzersLanguageLocalization, FormatAnalyzersRegionLocalization, FormatAnalyzersCore, FormatAnalyzersMaui, FormatAnalyzersSolution

# Synopsis: Format Analyzers Solution
Task FormatAnalyzersSolution Restore, {
    # $solution = Resolve-Path -Path 'TIKSN Framework.sln'
    # Exec { dotnet format analyzers --severity info --verbosity diagnostic $solution }
}

# Synopsis: Format Analyzers Language Localization
Task FormatAnalyzersLanguageLocalization Restore, {
    $project = Resolve-Path -Path 'TIKSN.LanguageLocalization/TIKSN.LanguageLocalization.csproj'

    Exec { dotnet format analyzers --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Analyzers Region Localization
Task FormatAnalyzersRegionLocalization Restore, {
    $project = Resolve-Path -Path 'TIKSN.RegionLocalization/TIKSN.RegionLocalization.csproj'

    Exec { dotnet format analyzers --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Analyzers Core
Task FormatAnalyzersCore Restore, {
    $project = Resolve-Path -Path 'TIKSN.Framework.Core/TIKSN.Framework.Core.csproj'

    Exec { dotnet format analyzers --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Analyzers MAUI
Task FormatAnalyzersMaui Restore, {
    $project = Resolve-Path -Path 'TIKSN.Framework.Maui/TIKSN.Framework.Maui.csproj'

    Exec { dotnet format analyzers --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Style
Task FormatStyle Restore, FormatStyleLanguageLocalization, FormatStyleRegionLocalization, FormatStyleCore, FormatStyleMaui, FormatStyleSolution

# Synopsis: Format Style Solution
Task FormatStyleSolution Restore, {
    # $solution = Resolve-Path -Path 'TIKSN Framework.sln'
    # Exec { dotnet format style --severity info --verbosity diagnostic $solution }
}

# Synopsis: Format Style Language Localization
Task FormatStyleLanguageLocalization Restore, {
    $project = Resolve-Path -Path 'TIKSN.LanguageLocalization/TIKSN.LanguageLocalization.csproj'

    Exec { dotnet format style --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Style Region Localization
Task FormatStyleRegionLocalization Restore, {
    $project = Resolve-Path -Path 'TIKSN.RegionLocalization/TIKSN.RegionLocalization.csproj'

    Exec { dotnet format style --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Style Core
Task FormatStyleCore Restore, {
    $project = Resolve-Path -Path 'TIKSN.Framework.Core/TIKSN.Framework.Core.csproj'

    Exec { dotnet format style --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Style MAUI
Task FormatStyleMaui Restore, {
    $project = Resolve-Path -Path 'TIKSN.Framework.Maui/TIKSN.Framework.Maui.csproj'

    Exec { dotnet format style --severity info --verbosity diagnostic $project }
}

# Synopsis: Format Whitespace
Task FormatWhitespace Restore, {
    $solution = Resolve-Path -Path 'TIKSN Framework.sln'
    Exec { dotnet format whitespace --verbosity diagnostic $solution }
}

# Synopsis: Download Currency Codes
Task DownloadCurrencyCodes Clean, {
    Invoke-WebRequest -Uri 'https://www.six-group.com/dam/download/financial-information/data-center/iso-currrency/lists/list-one.xml' -OutFile 'TIKSN.Framework.Core/Finance/Resources/TableA1.xml'
    Invoke-WebRequest -Uri 'https://www.six-group.com/dam/download/financial-information/data-center/iso-currrency/lists/list-three.xml' -OutFile 'TIKSN.Framework.Core/Finance/Resources/TableA3.xml'

    @(
        'TIKSN.Framework.Core/Finance/Resources/TableA1.xml',
        'TIKSN.Framework.Core/Finance/Resources/TableA3.xml'
    ) | ForEach-Object {
        $content = Get-Content -Path $_ -Raw
        $xml = [xml]$content
        $stringWriter = New-Object System.IO.StringWriter
        $xmlWriter = New-Object System.Xml.XmlTextWriter $stringWriter
        $xmlWriter.Formatting = 'Indented'
        $xmlWriter.Indentation = 4
        $xml.WriteContentTo($xmlWriter)
        $xmlWriter.Flush()
        $stringWriter.Flush()
        Set-Content -Path $_ -Value ($stringWriter.ToString())
    }
}

# Synopsis: Scan with DevSkim for security issues
Task DevSkim Restore, {
    $state = Import-Clixml -Path ".\.trash\$Instance\state.clixml"
    $trashFolder = $state.TrashFolder
    $sarifFile = Join-Path -Path $trashFolder -ChildPath 'DevSkim.sarif'
    Exec { dotnet tool run devskim analyze --source-code . --output-file $sarifFile }
    Exec { dotnet tool run devskim fix --source-code . --sarif-result $sarifFile --all }
}

# Synopsis: Restore
Task Restore RestoreWorkloads, RestoreTools, RestorePackages

# Synopsis: Restore workloads
Task RestoreWorkloads Clean, {
    Exec { dotnet workload restore }
}

# Synopsis: Restore tools
Task RestoreTools Clean, {
    Exec { dotnet tool restore }
}

# Synopsis: Restore packages
Task RestorePackages Clean, {
    $solution = Resolve-Path -Path 'TIKSN Framework.sln'
    Exec { dotnet restore $solution }
}

# Synopsis: Clean previous build leftovers
Task Clean Init, {
    Get-ChildItem -Directory
    | Where-Object { -not $_.Name.StartsWith('.') }
    | ForEach-Object { Get-ChildItem -Path $_ -Recurse -Directory }
    | Where-Object { ( $_.Name -eq 'bin') -or ( $_.Name -eq 'obj') }
    | ForEach-Object { Remove-Item -Path $_ -Recurse -Force }
}

# Synopsis: Initialize folders and variables
Task Init {
    $trashFolder = Join-Path -Path . -ChildPath '.trash'
    $trashFolder = Join-Path -Path $trashFolder -ChildPath $Instance
    New-Item -Path $trashFolder -ItemType Directory | Out-Null
    $trashFolder = Resolve-Path -Path $trashFolder

    $buildArtifactsFolder = Join-Path -Path $trashFolder -ChildPath 'artifacts'
    New-Item -Path $buildArtifactsFolder -ItemType Directory | Out-Null

    $anyBuildArtifactsFolder = Join-Path -Path $buildArtifactsFolder -ChildPath 'any'
    New-Item -Path $anyBuildArtifactsFolder -ItemType Directory | Out-Null

    $anyIosBuildArtifactsFolder = Join-Path -Path $buildArtifactsFolder -ChildPath 'any-ios'
    New-Item -Path $anyIosBuildArtifactsFolder -ItemType Directory | Out-Null

    $anyMaccatalystBuildArtifactsFolder = Join-Path -Path $buildArtifactsFolder -ChildPath 'any-maccatalyst'
    New-Item -Path $anyMaccatalystBuildArtifactsFolder -ItemType Directory | Out-Null

    $anyAndroidBuildArtifactsFolder = Join-Path -Path $buildArtifactsFolder -ChildPath 'any-android'
    New-Item -Path $anyAndroidBuildArtifactsFolder -ItemType Directory | Out-Null

    $anyWindowsBuildArtifactsFolder = Join-Path -Path $buildArtifactsFolder -ChildPath 'any-windows'
    New-Item -Path $anyWindowsBuildArtifactsFolder -ItemType Directory | Out-Null

    $state = [PSCustomObject]@{
        PackageId                          = 'TIKSN-Framework'
        NextVersion                        = $null
        TrashFolder                        = $trashFolder
        BuildArtifactsFolder               = $buildArtifactsFolder
        AnyBuildArtifactsFolder            = $anyBuildArtifactsFolder
        AnyIosBuildArtifactsFolder         = $anyIosBuildArtifactsFolder
        AnyMaccatalystBuildArtifactsFolder = $anyMaccatalystBuildArtifactsFolder
        AnyAndroidBuildArtifactsFolder     = $anyAndroidBuildArtifactsFolder
        AnyWindowsBuildArtifactsFolder     = $anyWindowsBuildArtifactsFolder
    }

    $state | Export-Clixml -Path ".\.trash\$Instance\state.clixml"
    Write-Output $state
}
