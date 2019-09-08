Task Publish -depends Pack {
}

Task Pack -depends Build {
}

Task Build -depends EstimateVersions {
}

Task EstimateVersions -depends Clean {
}

Task Clean -depends Init {
}

Task Init {
   $date = Get-Date
   $ticks = $date.Ticks
   $trashFolder = Join-Path -Path . -ChildPath ".trash"
   $script:trashFolder = Join-Path -Path $trashFolder -ChildPath $ticks.ToString("D19")
   New-Item -Path $script:trashFolder -ItemType Directory | Out-Null
   $script:trashFolder = Resolve-Path -Path $script:trashFolder

   $script:buildArtifactsFolder = Join-Path -Path $script:trashFolder -ChildPath "artifacts"
   New-Item -Path $script:buildArtifactsFolder -ItemType Directory | Out-Null

   $script:armBuildArtifactsFolder = Join-Path -Path $script:buildArtifactsFolder -ChildPath "any"
   New-Item -Path $script:armBuildArtifactsFolder -ItemType Directory | Out-Null

   $script:anyBuildArtifactsFolder = Join-Path -Path $script:buildArtifactsFolder -ChildPath "arm"
   New-Item -Path $script:anyBuildArtifactsFolder -ItemType Directory | Out-Null

   $script:x64BuildArtifactsFolder = Join-Path -Path $script:buildArtifactsFolder -ChildPath "x64"
   New-Item -Path $script:x64BuildArtifactsFolder -ItemType Directory | Out-Null

   $script:x86BuildArtifactsFolder = Join-Path -Path $script:buildArtifactsFolder -ChildPath "x86"
   New-Item -Path $script:x86BuildArtifactsFolder -ItemType Directory | Out-Null
}
