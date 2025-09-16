$rootFolder = $PSScriptRoot
$sourcePath = "$rootFolder/assets/datadictionary/fieldinfo.csv"
$destinationPath = "$rootFolder/assets/datadictionary/fieldinfo.json"

Write-Host "Reading field info from $sourcePath"
$data = Get-Content $sourcePath | ConvertFrom-Csv 

foreach($item in $data){
    $out = $null
    if([bool]::TryParse($item.IsMetric, [ref]$out)){
        $item.IsMetric = $out
    }
    if([bool]::TryParse($item.IsCurrency, [ref]$out)){
        $item.IsCurrency = $out
    }
    if([bool]::TryParse($item.IsCommon, [ref]$out)){
        $item.IsCommon = $out
    }
    if([bool]::TryParse($item.IsCommonField, [ref]$out)){
        $item.IsCommonField = $out
    }
    if([bool]::TryParse($item.IsCommonMetric, [ref]$out)){
        $item.IsCommonMetric = $out
    }
}

$json = $data | ConvertTo-Json
Write-Host "Converting to json at $destinationPath"
Set-Content -Path $destinationPath -Value $json
