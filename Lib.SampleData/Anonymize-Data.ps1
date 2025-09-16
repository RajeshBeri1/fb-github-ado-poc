#
# This script will replace brand and company identifiers from the sample data to allow testing foreach
# end users without permissions on specific accounts.
# 
# Note: Files in the content folder will be overwritten. It has been tested with the data as of 1/13/2022.
#       For additional data it will be necessary to add additional search/replace pairs.
#

$startFolder = $PSScriptRoot
$contentFolder = "$startFolder/Content/*"

$replacements = @{
 "Wholly"    = "Contoso"
 "MegaMex"   = "Fabrikam"
 "Guacamole" = "Frozen Yogurt"
 "Avocado"   = "Frozen Custard"
 "Pepperoni" = "Sorbet"
 "SPAM"      = "Gelato"
 "Bacon"     = "Fried Cheesecake"
 "Dinty Moore" = "Frankie's Sherbet"
 "Herdez"    = "NorthWind"
 "Planters"  = "Coho Winery"
 '"MMX"'     = '"FBK"'
 '"WG"'      = '"FY"'
 '"HO"'      = '"CO"'
 '"PLT"'     = '"CW"'
 '"HER"'     = '"NW"'
 '"SPM"'     = '"GLT"'
 '"hormel"'  = '"Contoso"'
}

Write-Host "Replacing data tokens in folder $contentFolder"

$files = Get-ChildItem $contentFolder -Filter data_*

Write-Host "Found $($files.Count) data files"

foreach ($file in $files) {
	Write-Host "Processing file '$file'"
	foreach ($searchString in $replacements.Keys) {
		Write-Host "Replacing '${searchString}' with '$($replacements.$searchString)' in file $file"

		(Get-Content $file) -Replace $searchString, $replacements.$searchString | Set-Content $file
	}
}

