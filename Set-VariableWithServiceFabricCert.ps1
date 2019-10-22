
#Login-AzureRmAccount
#Get-AzureRmSubscription | Out-GridView -PassThru | Set-AzureRmContext
#Get-AzureRmContext

#Requires -Version 3.0
Param(
    [Parameter(Mandatory=$false)] [string] $Service = 'ZZ',
    [Parameter(Mandatory=$false)] [string] $Environment = 'PC1',
    [Parameter(Mandatory=$false)] [string] $RegionCode = 'WEU',

    [Parameter(Mandatory=$false)] [string] $CertificateName = "ServiceFabricCertificate"
)

$KeyValutName = "$($Service)-$($Environment)-KeyVault-$($RegionCode)"

#$ResourceGroupName ="$($Service)-$($Environment)-KeyVault-$($RegionCode)"
#Write-Output "ResourceGroupName : $($ResourceGroupName)"

Write-Output "KeyValutName : $($KeyValutName)"
Write-Output "CertificateName : $($CertificateName)"

$AzureKeyVaultSecret=(Get-AzureKeyVaultSecret -VaultName $KeyValutName -Name $CertificateName -ErrorAction SilentlyContinue).SecretValueText

Write-Output "Secret : $($Secret)"


Write-Host "##vso[task.setvariable variable=ServiceFabricCertificateAsString123]AHHHHHHHHHHHH";
Write-Host "##vso[task.setvariable variable=ServiceFabricCertificateAsString]$($AzureKeyVaultSecret)";

