[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Version,
    [Parameter()]
    [string]
    $Instance
)

.\trigger.ps1 -Task Format -Instance $Instance -Version $Version
