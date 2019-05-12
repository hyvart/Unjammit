param(
	[string]
	$Path = $(Get-Location),

	# NuGet executable location
	[ValidateScript({Test-Path $_})]
	[string]
	$NugetExe = $(Get-Command nuget).Source,

	[string]
	$OutFile = $(Join-Path -Path $Path -ChildPath 'catalog.json')
)

Add-Type -AssemblyName System.IO.Compression.FileSystem
Add-Type -Path "$(& $NugetExe config repositoryPath)/plist-cil/1.60.0/lib/netstandard2.0/plist-cil.dll"

$instrumentIds = @{
	0 = 'Guitar';
	1 = 'Bass';
	2 = 'Drums';
	3 = 'Keyboard';
	4 = 'Vocal'
}

$zipFiles = Get-ChildItem -Path $Path -Filter *.zip
$processed = @()
$unprocessed = @()
foreach ($zipFile in $zipFiles) {
	try {
		$archive = [System.IO.Compression.ZipFile]::OpenRead($zipFile)
		$entry = $archive.Entries | Where-Object Name -eq 'info.plist'
		$songInfo = [Claunia.PropertyList.PropertyListParser]::Parse($entry.Open())

		$id = Split-Path -Path $zipFile -Leaf
		if ($id -like '*-*-*-*-*.zip' -and $id.Length -eq 40) {
			$id = $id.Substring(0, 36)
		}
		$instrumentId = [int]$songInfo['instrument']
		$object = [PSCustomObject] @{
			id         = $id
			sku        = $songInfo['sku'].Content
			artist     = $songInfo['artist'].Content
			album      = $songInfo['album'].Content
			title      = $songInfo['title'].Content
			instrument = $instrumentIds[$instrumentId]
			genre      = $songInfo['genre'].Content
		}

		$processed += $object
	}
	catch {
		$unprocessed += $zipFile
	}
}

[PSCustomObject] @{ songs = $processed } | ConvertTo-Json | Out-File -Encoding utf8 -FilePath $OutFile
Write-Output "Processed $($processed.Length) files."

if ($unprocessed.Length -gt 0) {
	Write-Output "$($unprocessed.Length) errors encountered:"
	Write-Output $unprocessed
}