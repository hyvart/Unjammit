param(
	[Parameter(Mandatory=$true)]
	[double] $BeatsPerMinute,

	[Parameter(Mandatory=$true)]
	[double] $Duration,

	[ValidateScript({Test-Path $_})]
	[System.IO.DirectoryInfo]
	$JcfPath,

	[double] $Offset = 0,

	[ValidateScript({Test-Path $_})]
	[System.IO.FileInfo]
	$NugetExe = $(Get-Command nuget).Source,

	[version]
	$PlistCilVersion = '2.1.0'
)

$packagesFolder = $(& $NugetExe config globalPackagesFolder)
Add-Type -Path $packagesFolder/plist-cil/$PlistCilVersion/lib/netstandard2.0/plist-cil.dll

$beats = [Claunia.PropertyList.NSArray]::new()

$position = $Offset
while($position -le $Duration) {
	$beat = New-Object -TypeName Claunia.PropertyList.NSDictionary
	$beat.Add('position', $position)
	$beats.Add($beat)
	$position += 60 / $BeatsPerMinute
}

if ($JcfPath) {
	[Claunia.PropertyList.PropertyListParser]::SaveAsXml($beats, "$JcfPath/beats.plist")
} else {
	Write-Host $beats
}
