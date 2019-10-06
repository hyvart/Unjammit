param(
	[string[]] $OtherArgs,

	[ValidateSet('x64', 'x86', 'ARM')]
	[string] $Platform = 'x64',

	[ValidateSet('Debug', 'Release')]
	[string] $Configuration = 'Debug',

	[System.IO.FileInfo] $Project = "$(Get-Location)\UWP\Unjammit.UWP.csproj"
)

msbuild $Project `
	"/p:Platform=$Platform" `
	"/p:Configuration=$Configuration" `
	"/p:AppxBundlePlatforms=$Platform" `
	"/p:AppxBundle=Always" `
	"/p:UapAppxPackageBuildMode=StoreUpload" `
	$OtherArgs
