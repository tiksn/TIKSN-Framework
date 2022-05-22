[CmdletBinding()]
param (
)

Invoke-psake -buildFile .\psakefile.ps1 -taskList Format
