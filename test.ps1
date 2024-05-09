[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Version = '0.1.0',
    [Parameter()]
    [string]
    $Instance
)

.\trigger.ps1 -Task Test -Instance $Instance -Version $Version
