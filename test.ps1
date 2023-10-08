[CmdletBinding()]
param (
)

Invoke-psake -buildFile .\psakefile.ps1 -taskList Test -parameters @{Version = '0.1.0' }
