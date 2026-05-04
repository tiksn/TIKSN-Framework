#requires -Version 7.6
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

.\trigger.ps1 -Task Pack -Instance $Instance -Version $Version
