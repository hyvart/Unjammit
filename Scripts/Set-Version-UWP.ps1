param(
	[Parameter(Mandatory=$true)]
	[ValidateScript({Test-Path $_})]
	[string] $AppxManifest,

	[Parameter(Mandatory=$true)]
	[string] $PackageIdentityName,

	[Parameter(Mandatory=$true)]
	[string] $PackageDisplayName,

	[Parameter(Mandatory=$true)]
	[version] $Version
)

$manifest = Get-ChildItem -Path $AppxManifest

$xml = New-Object -TypeName System.Xml.XmlDocument
$xml.PreserveWhitespace = $true
$xml.Load($manifest)

$xml.Package.Identity.Name = $PackageIdentityName
$xml.Package.Properties.DisplayName = $PackageDisplayName
$xml.Package.Identity.Version = "$Version"

$xml.Save($manifest)