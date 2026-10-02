param()
$ErrorActionPreference = 'Stop'
$dependencyRoot = Join-Path (Split-Path $PSScriptRoot -Parent) '.dependencies'
$packageFolder = Join-Path $dependencyRoot 'Thorn_Core-0.5.0'
$archivePath = Join-Path $dependencyRoot 'Thorn_Core-0.5.0.zip'
New-Item -ItemType Directory -Path $dependencyRoot -Force | Out-Null
Invoke-WebRequest -Uri 'https://thunderstore.io/package/download/end_4/Thorn_Core/0.5.0/' -OutFile $archivePath -UseBasicParsing
Expand-Archive -LiteralPath $archivePath -DestinationPath $packageFolder -Force
$library = Get-ChildItem -LiteralPath $packageFolder -Recurse -Filter 'ThornClient.dll' | Select-Object -First 1
if ($null -eq $library) { throw 'ThornClient.dll was not found in the downloaded package.' }
Copy-Item -LiteralPath $library.FullName -Destination (Join-Path $dependencyRoot 'ThornClient.dll') -Force
Write-Host 'Thorn Core 0.5.0 build reference downloaded. Install the complete mod and its dependencies separately to run ReEdited.'
