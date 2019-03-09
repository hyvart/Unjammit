param(
	[string] $Xcf,

	[string] $SourceDir,

	[string] $TargetDir,

	[int] $IndexOffset = 0
)

Add-Type -AssemblyName System.Drawing

foreach($relPath in (Get-ChildItem -Name -Recurse -Include '*.png' $SourceDir)) {
	$file = Get-ChildItem (Join-Path -Path $SourceDir -ChildPath $relPath)
	$bmp = [System.Drawing.Bitmap] $file.FullName

	$width = $bmp.Width
	$height = $bmp.Height

	$index = $IndexOffset
	if ($width -eq $height) {
		$index += 0
	} else {
		$index += 4
	}

	magick convert $Xcf"[$index]" -resize ${width}x${height} (Join-Path -Path $TargetDir -ChildPath $file.Name)
}
