param(
	[Parameter(Mandatory=$true)]
	[System.IO.FileInfo[]]
	$Sources,

	[Parameter(Mandatory=$true)]
	[System.IO.DirectoryInfo]
	$Destination,

	[Parameter(Mandatory=$true)]
	[guid]
	$Guid,

	[ValidateSet('t', 'n')]
	[char]
	$Type = 'n',

	[ValidateSet('Q16-HDRI', 'Q16', 'Q8')]
	[string]
	$MagickNetVariant = 'Q8',

	[version]
	$MagickNetVersion = '7.19.0.1'
)

$globalPackagesFolder = $(nuget config globalPackagesFolder)
$packageName = "Magick.NET-${MagickNetVariant}-AnyCPU"

#TODO: temporary native symlink
$arch = "$([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)".ToLower()
[string] $os
[string] $ext
if ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows)) {
	$os = 'win'
	$ext = 'dll'
} elseif ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::OSX)) {
	$os = 'osx'
	$ext = 'dylib'
} elseif ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Linux)) {
	$os = 'linux'
	$ext = 'so'
} else {
	Write-Error "Unidentified OS platform."
	Exit
}

New-Item -ItemType SymbolicLink `
	-Name "Magick.Native-${MagickNetVariant}-${arch}.${ext}" `
	-Value (Join-Path -Path $globalPackagesFolder -ChildPath $packageName -AdditionalChildPath $MagickNetVersion, 'runtimes', "${os}-${arch}", 'native', "Magick.Native-${MagickNetVariant}-${arch}.${ext}") `
	-ErrorAction Stop

 Add-Type `
 	-Path	(Join-Path -Path $globalPackagesFolder -ChildPath 'Magick.NET.Core' -AdditionalChildPath '2.0.0', 'lib', 'netstandard20', 'Magick.NET.Core.dll'), `
	 		(Join-Path -Path $globalPackagesFolder -ChildPath $packageName -AdditionalChildPath $MagickNetVersion, 'lib', 'netstandard20', "${packageName}.dll") `
 	-ErrorAction Stop

$count = 0
$Sources | ForEach-Object {
	Write-Host 'Processing ' $_.FullName
	$svg = [ImageMagick.MagickImage]::new($_)
	$svg.Resize(724, 1024)
	$svg.Write(( Join-Path -Path $Destination -ChildPath "${Guid}_jcf${Type}_$("$count".PadLeft(2, '0'))" ))

	$count = $count + 1
}

Remove-Item -Path "$(Get-Location)/Magick.Native-${MagickNetVariant}-${arch}.${ext}"
