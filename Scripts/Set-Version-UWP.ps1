param(
	[Parameter(Mandatory=$true)]
	[ValidateScript({Test-Path $_})]
	[string] $AppxManifest,

	[string] $PackageIdentityName,

	[string] $PackageDisplayName,

	[Parameter(Mandatory=$true)]
	[version] $Version
)

$manifest = Get-ChildItem -Path $AppxManifest

$xml = New-Object -TypeName System.Xml.XmlDocument
$xml.PreserveWhitespace = $true
$xml.Load($manifest)

if ($PackageIdentityName) {
	$xml.Package.Identity.Name = $PackageIdentityName
}
if ($PackageDisplayName) {
	$xml.Package.Properties.DisplayName = $PackageDisplayName
	$xml.Package.Applications.Application.VisualElements.DisplayName = $PackageDisplayName
}

$xml.Package.Identity.Version = "$Version"

$xml.Save($manifest)