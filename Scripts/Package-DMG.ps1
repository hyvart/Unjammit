param (
	[ValidateScript({Test-Path $_})]
	[string] $SrcFolder = "$(Split-Path $PSScriptRoot)/Target/iPhoneSimulator/$Configuration/Unjammit.macOS",

	[string] $Version = '0.0.0',

	[ValidateSet('Debug', 'Release')]
	[string] $Configuration = 'Release',

	[string] $OutputName = 'Unjammit',

	[ValidateScript({Test-Path $_})]
	[string] $OutputPath = "$(Split-Path $PSScriptRoot)/Target/iPhoneSimulator/$Configuration/Unjammit.macOS/${OutputName}.dmg"
)

hdiutil create `
	-format UDZO `
	-srcfolder $SrcFolder `
	"$OutputPath/$OutputName-$Version.dmg"
