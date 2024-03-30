[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Version
)

Invoke-Build -Task Publish -Version $Version
