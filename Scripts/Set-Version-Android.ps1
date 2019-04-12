param(
	[Parameter(Mandatory=$true)]
	[ValidateScript({Test-Path $_})]
	[string] $Manifest,

	[string] $PackageName,

	[string] $Label,

	[string] $VersionCode,

	[Parameter(Mandatory=$true)]
	[version] $VersionName
)

$manifest = Get-ChildItem -Path $Manifest

$xml = New-Object -TypeName System.Xml.XmlDocument
$xml.PreserveWhitespace = $true
$xml.Load($manifest)

if ($VersionCode) {
	$xml.manifest.versionCode = $VersionCode
}
if ($PackageName) {
	$xml.manifest.package  = $PackageName
}
if ($Label) {
	$xml.manifest.application.label = $Label
}

$xml.manifest.versionName = "$VersionName"

$xml.Save($manifest)