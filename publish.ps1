Param(
    [string]$Version
)

Invoke-psake -buildFile .\psakefile.ps1 -taskList Tweet
