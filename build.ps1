[CmdletBinding()]
param (
)

Invoke-psake -buildFile .\psakefile.ps1 -taskList Build -parameters @{Version = '0.1.0' }
