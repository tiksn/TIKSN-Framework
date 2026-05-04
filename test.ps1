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

.\trigger.ps1 -Task Test -Instance $Instance -Version $Version
