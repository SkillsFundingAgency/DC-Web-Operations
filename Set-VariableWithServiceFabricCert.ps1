
#Login-AzureRmAccount
#Get-AzureRmSubscription | Out-GridView -PassThru | Set-AzureRmContext
#Get-AzureRmContext

#Requires -Version 3.0
Param(
    #[Parameter(Mandatory=$false)] [string] $Service = 'ZZ',
    #[Parameter(Mandatory=$false)] [string] $Environment = 'PC1',
    #[Parameter(Mandatory=$false)] [string] $RegionCode = 'WEU',

    [Parameter(Mandatory=$false)] [string] $Service = 'DCOL',
    [Parameter(Mandatory=$false)] [string] $Environment = 'DEVCI',
    [Parameter(Mandatory=$false)] [string] $RegionCode = 'WEU',

    [Parameter(Mandatory=$false)] [string] $CertificateName = "ServiceFabricCertificate"
)

$KeyValutName = "$($Service)-$($Environment)-KeyVault-$($RegionCode)"

#$ResourceGroupName ="$($Service)-$($Environment)-KeyVault-$($RegionCode)"
#Write-Output "ResourceGroupName : $($ResourceGroupName)"

Write-Output "KeyValutName : $($KeyValutName)"
Write-Output "CertificateName : $($CertificateName)"
Write-Output ""
Write-Output ""

$cert = Get-AzureKeyVaultSecret -VaultName $KeyValutName -Name $CertificateName -ErrorAction SilentlyContinue
#$cert.SecretValueText
Write-Output ""
Write-Output ""
Write-Output ""

Write-Output "Cert Expires : $($cert.Expires)"

Write-Host "##vso[task.setvariable variable=ServiceFabricCertificateAsString]$($cert.SecretValueText)";

Write-Output ""
Write-Output ""
Write-Output ""
Write-Output "$($AzureKeyVaultSecret)"
Write-Output ""
Write-Output ""



