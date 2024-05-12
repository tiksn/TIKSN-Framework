[CmdletBinding()]
[System.Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSAvoidUsingCmdletAliases', '', Justification = 'It will not work without the aliases.')]
param (
    [Parameter()]
    [string]
    $Task,
    [Parameter()]
    [string]
    $Instance,
    [Parameter()]
    [string]
    $Version
)

if ([System.String]::IsNullOrWhiteSpace($Instance)) {
    $Instance = Get-Date -Format 'yyyyMMddHHmmss'
    Invoke-Build -Task $Task -Instance $Instance -Version $Version
}
else {
    $trashFolder = Join-Path -Path . -ChildPath '.trash'
    $trashFolder = Join-Path -Path $trashFolder -ChildPath $date
    if (-not (Test-Path -Path $trashFolder)) {
        New-Item -Path $trashFolder -ItemType Directory | Out-Null
    }
    $checkpointFile = Join-Path -Path $trashFolder -ChildPath "$Instance.clixml"
    if (Test-Path -Path $checkpointFile) {
        $checkpointData = Import-Clixml -Path $checkpointFile
        $checkpointData.Task = @($Task)
        $checkpointData | Export-Clixml -Path $checkpointFile
    }
    Build-Checkpoint -Checkpoint $checkpointFile -Build @{Task = $Task; Instance = $Instance; Version = $Version } -Preserve -Auto
}
