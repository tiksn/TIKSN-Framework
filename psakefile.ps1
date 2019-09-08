Task Publish -depends Pack {
}

Task Pack -depends Build, EstimateVersions {
}

Task EstimateVersions {
}

Task Build -depends Clean {
}

Task Clean -depends Init {
}

Task Init {
   $date = Get-Date
   $ticks = $date.Ticks
   $trashFolder = Join-Path -Path . -ChildPath ".trash"
   $script:trashFolder = Join-Path -Path $trashFolder -ChildPath $ticks.ToString("D19")
   New-Item -Path $script:trashFolder -ItemType Directory
   $script:trashFolder = Resolve-Path -Path $script:trashFolder
}
