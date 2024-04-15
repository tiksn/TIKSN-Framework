[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Version = '0.1.0',
    [Parameter()]
    [string]
    $Instance
)

.\trigger.ps1 -Task Restore -Instance $Instance -Version $Version
