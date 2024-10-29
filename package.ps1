$projectFile = "TailscaleClient.csproj"
$outputDir = "./Dist"
$thumbprint = "9C3E8CCAC08122B756BC98E5559841BE87816AAA"
$baseUri = "https://tsc.xirreal.dev"

Write-Output "Building MSIX packages..."
dotnet restore $projectFile
dotnet msbuild $projectFile -t:Rebuild -p:Platform=x64 -p:Configuration=Release -p:OutDir="$outputDir/x64/"
dotnet msbuild $projectFile -t:Rebuild -p:Platform=arm64 -p:Configuration=Release -p:OutDir="$outputDir/arm64/"

makeappx.exe pack /h SHA256 /d ".\Dist\x64" /o /p "./Dist/TailscaleClient_x64.msix"
signtool.exe sign /fd SHA256 /sha1 "$thumbprint" /t http://timestamp.digicert.com "./Dist/TailscaleClient_x64.msix"
makeappx.exe pack /h SHA256 /d ".\Dist\arm64" /o /p "./Dist/TailscaleClient_arm64.msix"
signtool.exe sign /fd SHA256 /sha1 "$thumbprint" /t http://timestamp.digicert.com "./Dist/TailscaleClient_arm64.msix"

$cert = Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object { $_.Thumbprint -eq $thumbprint }
$certPath = "$outputDir/TailscaleClient.cer"
Export-Certificate -Cert $cert -FilePath $certPath

$appxManifest = Join-Path -Path $outputDir -ChildPath "x64\AppxManifest.xml"
[xml]$manifestXml = Get-Content $appxManifest
$identityNode = $manifestXml.Package.Identity
$publisher = $identityNode.Publisher
$version = $identityNode.Version
$appName = $identityNode.Name

Write-Output "Creating .appinstaller file..."
$appInstallerFile = "$outputDir/TailscaleClient.appinstaller"
$appInstallerXml = @"
<?xml version="1.0" encoding="utf-8"?>
<AppInstaller Uri="$baseUri/TailscaleClient.appinstaller" Version="$version" xmlns="http://schemas.microsoft.com/appx/appinstaller/2018">
    <MainPackage Name="$appName" Publisher="$publisher" Version="$version" Uri="$baseUri/TailscaleClient_x64.msix" ProcessorArchitecture="x64" />
    <MainPackage Name="$appName" Publisher="$publisher" Version="$version" Uri="$baseUri/TailscaleClient_arm64.msix" ProcessorArchitecture="arm64" />
    <UpdateSettings>
        <OnLaunch HoursBetweenUpdateChecks="0" />
        <AutomaticBackgroundTask />
        <ShowPrompt />
        <UpdateBlocksActivation />
    </UpdateSettings>
</AppInstaller>
"@
$appInstallerXml | Out-File -FilePath $appInstallerFile -Encoding utf8
Write-Output "AppInstaller file created at $appInstallerFile"

Remove-Item -Path "$outputDir\x64" -Recurse -Force
Remove-Item -Path "$outputDir\arm64" -Recurse -Force

Write-Output "Process completed successfully."
