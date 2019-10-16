param(
	[Parameter(Mandatory=$true)]
	[System.IO.FileInfo] $InfoPlist,

	[string] $BundleId,

	[string] $BundleName,

	[string] $BundleDisplayName,

	[Parameter(Mandatory=$true)]
	[version] $BundleShortVersionString,

	[string] $BundleVersion,

	[System.IO.FileInfo] $EntitlementsPlist,

	[string] $TeamPrefix,

	[string] $AssemblyName,

	[System.IO.FileInfo] $Project,

	[switch] $UsePlistCil,

	[string] $DownloadDir = "$($PSScriptRoot | Split-Path)\Build"
)

$plist = Get-ChildItem -Path $InfoPlist

if ($UsePlistCil) {
	# Install plist-cil
	if (! (Test-Path "$DownloadDir\plist-cil.2.0.0\lib\netstandard2.0\plist-cil.dll")) {
		if (! (Test-Path $DownloadDir)) {
			New-Item -ItemType Directory $DownloadDir
		}

		nuget install plist-cil -Version 2.0.0 -OutputDirectory $DownloadDir
	}

	Add-Type -Path "$DownloadDir\plist-cil.2.0.0\lib\netstandard2.0\plist-cil.dll"

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

	#TODO: Set custom entitlements

	[Claunia.PropertyList.PropertyListParser]::SaveAsXml($dict, $plist)
} else {
	if ($BundleId) {
		/usr/libexec/PlistBuddy -c "Set :CFBundleIdentifier '${BundleId}'" $InfoPlist
	}

	if ($BundleName) {
		/usr/libexec/PlistBuddy -c "Set :CFBundleName '${BundleName}'" $InfoPlist
	}
	
	if ($BundleDisplayName) {
		/usr/libexec/PlistBuddy -c "Set :CFBundleDisplayName '${BundleDisplayName}'" $InfoPlist
	}
	
	if ($BundleVersion) {
		/usr/libexec/PlistBuddy -c "Set :CFBundleVersion '${BundleVersion}'" $InfoPlist
	}
	
	/usr/libexec/PlistBuddy -c "Set :CFBundleShortVersionString '${BundleShortVersionString}'" $InfoPlist

	if ($EntitlementsPlist -and $TeamPrefix) {
		/usr/libexec/PlistBuddy -c "Set :com.apple.application-identifier '${TeamPrefix}.${BundleId}'" $EntitlementsPlist
	}
}

if ($AssemblyName -and $Project) {
	$xml = New-Object -TypeName System.Xml.XmlDocument
	$xml.PreserveWhitespace = $true
	$xml.Load((Get-ChildItem $Project))

	($xml.Project.PropertyGroup | Where-Object -Property AssemblyName).AssemblyName = $AssemblyName

	$xml.Save((Get-ChildItem $Project))
}
