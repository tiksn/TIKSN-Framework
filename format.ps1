[CmdletBinding()]
param (
)

Invoke-psake -buildFile .\psakefile.ps1 -taskList Format -parameters @{Version = '0.1.0' }
