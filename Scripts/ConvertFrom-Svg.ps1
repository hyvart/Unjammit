param(

	[Parameter(Mandatory=$true)]
	[System.IO.DirectoryInfo]
	$Source,

	[Parameter(Mandatory=$true)]
	[System.IO.DirectoryInfo]
	$Destination,

	[Parameter(Mandatory=$true)]
	[guid]
	$Guid,

	[ParameterSet('t', 'n')]
	[char]
	$Type = 'n',

	[ParameterSet('Q16-HDRI', 'Q16', 'Q8')]
	[string]
	$MagickNetVariant = 'Q8',

	[version]
	$MagickNetVersion = '7.19.0.1'
)

$globalPackagesFolder = $(nuget config globalPackagesFolder)
$packageName = "Magick.NET-${MagickNetVariant}-AnyCPU"

#TODO: temporary native symlink

Add-Type -Path `
	"$globalPackagesFolder/Magick.NET.Core/2.0.0/lib/netstandard20/Magick.NET.Core.dll" `
	"$globalPackagesFolder/$packageName/$MagickNetVersion/lib/netstandard20/${packageName}.dll"

$files = Get-ChildItem $Source -Include *.svg

foreach ($file in $files) {
	$svg = new [ImageMagick.MagickImage]::new($file, [ImageMagick.MagickFormat].Svg)
	$svg.Resize(724, 1024)
}
