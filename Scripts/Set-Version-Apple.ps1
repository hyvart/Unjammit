param(
	[Parameter(Mandatory=$true)]
	[ValidateScript({Test-Path $_})]
	[string] $InfoPlist,

	[string] $BundleId,

	[string] $BundleName,

	[string] $BundleDisplayName,

	[Parameter(Mandatory=$true)]
	[version] $BundleShortVersionString,

	[version] $BundleVersion,

	[string] $DownloadDir = "$($PSScriptRoot | Split-Path)\Build"
)

$plist = Get-ChildItem -Path $InfoPlist

# Install plist-cil
if (! (Test-Path "$DownloadDir\plist-cil.1.50.0\lib\netstandard2.0\plist-cil.dll")) {
	if (! (Test-Path $DownloadDir)) {
		New-Item -ItemType Directory $DownloadDir
	}

	nuget install plist-cil -Version 1.50.0 -OutputDirectory $DownloadDir
}

Add-Type -Path "$DownloadDir\plist-cil.1.50.0\lib\netstandard2.0\plist-cil.dll"

$dict = [Claunia.PropertyList.PropertyListParser]::Parse($plist)

if ($BundleId) {
	$dict['CFBundleIdentifier'].Content = $BundleId
}
if ($BundleName) {
	$dict['CFBundleName'].Content = $BundleName
}
if ($BundleDisplayName) {
	$dict['CFBundleDisplayName'].Content = $BundleDisplayName
}

$dict['CFBundleShortVersionString'].Content = $BundleShortVersionString

if ($BundleVersion) {
	$dict['CFBundleVersion'].Content = $BundleVersion
}

[Claunia.PropertyList.PropertyListParser]::SaveAsXml($dict, $plist)