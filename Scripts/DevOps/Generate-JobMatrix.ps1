<#
	Generates a matrix as defined by the Azure Pipelines schema.
	Outputs results in JSON format.
#>
param(
	[Parameter(Mandatory=$true)]
	[System.Collections.Specialized.OrderedDictionary] $Matrix,

	[switch] $Compress
)

$total = 1;
foreach ($k in $Matrix.Keys) {
	$total = $total * $Matrix[$k].Count
}

$result = @{}
[string[]]$keys = $Matrix.Keys
foreach($i in 0..($total-1)) {
	$m = 1
	$values = @()
	foreach ($j in ($keys.Count-1)..0) {
		$values = @($Matrix[ $keys[$j] ][ [Math]::Floor($i / $m) % $Matrix[$keys[$j]].Count ]) + $values
		$m = $m * $Matrix[$keys[$j]].Count
	}

	$newKey = $values -join '|'
	$result[$newKey] = @{}
	foreach($j in 0..($keys.Count-1)) {
		$result[$newKey][$keys[$j]] = $values[$j]
	}
}

return $result | ConvertTo-Json -Compress:$Compress
