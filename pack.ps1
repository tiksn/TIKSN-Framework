[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Version
)

Invoke-Build -Task Pack -Version $Version
