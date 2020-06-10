param(
	[Parameter(Mandatory=$true)]
	[ValidateScript({Test-Path $_})]
	[System.IO.DirectoryInfo]
	$JcfPath,

	[ValidateScript({Test-Path $_})]
	[System.IO.FileInfo]
	$NugetExe = $(Get-Command nuget).Source,

	[version]
	$PlistCilVersion = '2.1.0'
)

$packagesFolder = $(& $NugetExe config globalPackagesFolder)
Add-Type -Path (
	Join-Path `
		-Path $packagesFolder `
		-ChildPath 'plist-cil' `
		-AdditionalChildPath $PlistCilVersion, 'lib', 'netstandard2.0', 'plist-cil.dll'
	)

$tracks = [Claunia.PropertyList.PropertyListParser]::Parse( (Join-Path "$JcfPath" -ChildPath "tracks.plist") )
# We'll deal with this when the script is run on  Big Endian machine.
#$trackCount = [System.BitConverter]::GetBytes($tracks.Count - 2)
# if (! [System.BitConverter]::IsLittleEndian) {
# 	[byte[]]::Reverse($trackCount)
# }
$beats = [Claunia.PropertyList.PropertyListParser]::Parse( (Join-Path "$JcfPath" -ChildPath "beats.plist") )
$nodes = New-Object -TypeName byte[] -ArgumentList 0 #-ArgumentList (4 + 32 + 32 + ($tracks.Count - 3) * (4 + ($beats.Count * 2 * 2 * 2)))

$nodes +=  [System.BitConverter]::GetBytes($tracks.Count - 3)

# 0-based instrument track count
(0..($tracks.Count - 3 - 1)) | ForEach-Object {
	if ( (Join-Path -Path $JcfPath -ChildPath "$($tracks[${_}]['identifier'].Content)_jcfn*" | Get-ChildItem).Count ) {
		$nodes += [System.Text.Encoding]::ASCII.GetBytes( $tracks[$_]['title'].Content ) + [byte[]]::new(32 - ($tracks[$_]['title'].Content.Length))
		$nodes += [System.Text.Encoding]::ASCII.GetBytes('Score') + [byte[]]::new(32 - 'Score'.Length)

		#TODO: Actual nodes
		$nodes += [System.BitConverter]::GetBytes($beats.Count)
		(0 .. ($beats.Count - 1)) | ForEach-Object {
			$nodes += [System.BitConverter]::GetBytes( [ushort] 0 )
			$nodes += [System.BitConverter]::GetBytes( [ushort] ($_ / 8) )
			$nodes += [System.BitConverter]::GetBytes( [float] (672 / 8 * ($_ % 8) + 80) )
		}
	}

	if ( (Join-Path -Path $JcfPath -ChildPath "$($tracks[${_}]['identifier'].Content)_jcft*" | Get-ChildItem).Count ) {
		# Tablature detected. Duplicate nodes count
		[System.BitConverter]::GetBytes( ($tracks.Count-3) * 2 ).CopyTo($nodes, 0)

		$nodes += [System.Text.Encoding]::ASCII.GetBytes( $tracks[$_]['title'].Content ) + [byte[]]::new(32 - ($tracks[$_]['title'].Content.Length))
		$nodes += [System.Text.Encoding]::ASCII.GetBytes('Tablature') + [byte[]]::new(32 - 'Tablature'.Length)

		#TODO: Actual nodes
		$nodes += [System.BitConverter]::GetBytes($beats.Count)
		(0 .. ($beats.Count - 1)) | ForEach-Object {
			$nodes += [System.BitConverter]::GetBytes( [ushort] 0 )
			$nodes += [System.BitConverter]::GetBytes( [ushort] ($_ / 8) )
			$nodes += [System.BitConverter]::GetBytes( [float] (672 / 8 * ($_ % 8) + 80) )
		}
	}
}

[System.IO.File]::WriteAllBytes((Join-Path -Path $JcfPath -ChildPath 'nowline.nodes'), $nodes)
