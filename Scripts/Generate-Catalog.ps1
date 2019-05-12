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
	0 = 'guitar';
	1 = 'bass';
	2 = 'drums';
	3 = 'keyboard';
	4 = 'vocal'
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
		$object = @{
			'id' = $id
			'artist' = $songInfo['artist'].Content
			'album' = $songInfo['album'].Content
			'title' = $songInfo['title'].Content
			'genre' = $songInfo['genre'].Content
			'instrumentId' = $instrumentId
			'instrument' = $instrumentIds[$instrumentId]
			'sku' = $songInfo['sku'].Content
		}

		$processed += $object
	}
	catch {
		$unprocessed += $zipFile
	}
}

Out-File -Encoding utf8 -FilePath $OutFile -InputObject (ConvertTo-Json $processed)
Write-Output "Processed $($processed.Length) files."

if ($unprocessed.Length -gt 0) {
	Write-Output "$($unprocessed.Length) errors encountered:"
	Write-Output $unprocessed
}