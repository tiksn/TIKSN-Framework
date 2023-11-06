[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Version
)

Invoke-psake -buildFile .\psakefile.ps1 -taskList Pack -parameters @{Version = $Version }
