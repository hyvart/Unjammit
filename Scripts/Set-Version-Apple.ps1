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

	[ValidateScript({Test-Path $_})]
	[string] $EntitlementsPlist,

	[string] $TeamPrefix,

	[string] $DownloadDir = "$($PSScriptRoot | Split-Path)\Build",

	[switch] $UsePlistCil
)

$plist = Get-ChildItem -Path $InfoPlist

if ($UsePlistCil) {
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