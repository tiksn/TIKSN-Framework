Properties {
    $PackageId = 'TIKSN-Framework'
}

Task Publish -depends Pack {
}

Task Pack -depends Build {
}

Task Build -depends BuildLanguageLocalization, BuildRegionLocalization, BuildCommonCore, BuildNetCore, BuildNetFramework, BuildAndroid, BuildUWP, CreateReferenceAssembliesForUWP {
}

Task BuildLanguageLocalization -depends EstimateVersions {
    $project = Resolve-Path -Path 'TIKSN.LanguageLocalization/TIKSN.LanguageLocalization.csproj'

    Exec { dotnet build $project /p:Configuration=Release /p:version=$Script:NextVersion /p:OutDir=$script:anyBuildArtifactsFolder }
}

Task BuildRegionLocalization -depends EstimateVersions
Task BuildCommonCore -depends EstimateVersions
Task BuildNetCore -depends EstimateVersions
Task BuildNetFramework -depends EstimateVersions
Task BuildAndroid -depends EstimateVersions
Task BuildUWP -depends EstimateVersions
Task CreateReferenceAssembliesForUWP -depends EstimateVersions

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
