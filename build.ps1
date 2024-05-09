[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Version,
    [Parameter()]
    [string]
    $Instance
)

.\trigger.ps1 -Task Build -Instance $Instance -Version $Version
