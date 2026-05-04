#requires -Version 7.4
#requires -PSEdition Core

[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Version,
    [Parameter()]
    [string]
    $Instance
)

.\trigger.ps1 -Task Restore -Instance $Instance -Version $Version
