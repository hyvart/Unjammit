param(
	[Parameter(Mandatory=$true)]
	[ValidateScript({Test-Path $_})]
	[string] $AppxManifest,

	[string] $PackageIdentityName,

	[string] $PackageDisplayName,

	[Parameter(Mandatory=$true)]
	[version] $Version,

	[ValidateScript({Test-Path $_})]
	[string] $AssetsSource,

	[ValidateScript({Test-Path $_})]
	[string] $AssetsTarget
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

# Replace assets?
if ($AssetsSource -and $AssetsTarget) {
	Copy-Item -Force -Recurse (Join-Path $AssetsSource -ChildPath *) $AssetsTarget
}
#.\Scripts\Set-Version-UWP.ps1 -AppxManifest .\UWP\Package.appxmanifest -Version '0.0.7.0' -AssetsSource .\Assets\Alpha\UWP\ -AssetsTarget .