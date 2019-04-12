param(
	[string] $Xcf,

	[string] $SourceDir,

	[string] $TargetDir,

	[int] $IndexOffset = 0
)

$indexes = @{
	'Icon.png' = 0;
	'launcher_foreground.png' = 4;
}

foreach ($relPath in (Get-ChildItem -Name -Recurse -Include '*.png' $SourceDir)) {
	$file = Get-ChildItem "$SourceDir\$relPath"
	$bmp = [System.Drawing.Bitmap] $file.FullName
	$index = $indexes[$file.Name] + $IndexOffset
	$size = [string] $bmp.Width + "x" + $bmp.Height

	magick convert $Xcf"[$index]" -resize $size $TargetDir\$relPath
}