param(
	[string] $Xcf,

	[string] $SourceDir,

	[string] $TargetDir,

	[int] $IndexOffset = 0
)

Add-Type -AssemblyName System.Drawing

# From https://rosettacode.org/wiki/Least_common_multiple#version_2
function gcd ($a, $b)  {
    function pgcd ($n, $m)  {
        if($n -le $m) { 
            if($n -eq 0) {$m}
            else{pgcd $n ($m%$n)}
        }
        else {pgcd $m $n}
    }
    $n = [Math]::Abs($a)
    $m = [Math]::Abs($b)
    (pgcd $n $m)
}

function ratio([System.Drawing.Bitmap] $bmp) {
	$gcd = gcd $bmp.Width $bmp.Height

	return "$($bmp.Width / $gcd)x$($bmp.Height / $gcd)"
}

$indexes = @{
	'1x1' = 0;
	'2x3' = 1;
	'40x71' = 2;
	'192x251' = 3;
}

foreach ($relPath in (Get-ChildItem -Name -Recurse -Include '*.png','iTunesArtwork*' $SourceDir)) {
	$file = Get-ChildItem (Join-Path -Path $SourceDir -ChildPath $relPath)
	$bmp = [System.Drawing.Bitmap] $file.FullName
	$width = $bmp.Width
	$height = $bmp.Height

	$ratio = ratio $bmp
	$index = $indexes[$ratio] + $IndexOffset

	magick convert $Xcf"[$index]" -resize ${width}x${height} (Join-Path -Path $TargetDir -ChildPath $file.Name)
}