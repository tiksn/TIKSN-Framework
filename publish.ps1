Param(
    [string]$Version
)

Invoke-psake -buildFile .\psakefile.ps1 -taskList Publish -parameters @{Version = $Version}
