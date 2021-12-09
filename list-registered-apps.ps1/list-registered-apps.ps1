param()

# see: https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_preference_variables?view=powershell-7.1#erroractionpreference
$ErrorActionPreference = 'Stop'

$RegisteredApplications = Get-Item -Path HKCU:\SOFTWARE\RegisteredApplications -ErrorAction Stop | Select-Object -ExpandProperty Property
Write-Host ("Found {0} registered applications" -f @($RegisteredApplications.Count))
Write-Host ""

# Removed in PWSH 7.1
# see: https://github.com/PowerShell/PowerShell/issues/13042 https://github.com/dotnet/runtime/issues/37672
# $ResourceLoaderClass = [Windows.ApplicationModel.Resources.ResourceLoader, Windows.ApplicationModel.Resources, ContentType = WindowsRuntime]
# $appres = $ResourceLoaderClass::GetForCurrentView()

# see: https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/add-type?view=powershell-7.1
Add-Type -Path $(Join-Path $PSScriptRoot ".\winapi.cs") | Out-Null

class AppVO
{
    [string]$entry;
    [string]$clsPath;
    [string]$name;
    [string]$desc;
}

$appVOs = [Collections.Generic.List[AppVO]]::new();
foreach ($entryName in $RegisteredApplications) {
    $appKey = Get-ItemPropertyValue -Path HKCU:\SOFTWARE\RegisteredApplications -Name $entryName
    if (Test-Path -Path "HKCU:\$appKey") {
        $appName = Get-ItemPropertyValue -Path "HKCU:\$appKey" -Name "ApplicationName"
        $appDesc = Get-ItemPropertyValue -Path "HKCU:\$appKey" -Name "ApplicationDescription"
        if ($appName.StartsWith("@")) {
            $appName = [My.Spec]::LoadIndirectString($appName);
        }
        if ($appDesc.StartsWith("@")) {
            $appDesc = [My.Spec]::LoadIndirectString($appDesc);
        }
        # Write-Host "Found: $appKey"
        # Write-Host "`t$appName"
        # Write-Host "`t$appDesc"

        $vo = [AppVO]::new();
        $vo.entry = $entryName;
        $vo.clsPath = $appKey;
        $vo.name = $appName;
        $vo.desc = $appDesc;
        $appVOs.Add($vo);
    }
    else {
        Write-Host "NotFound: $appKey"
    }
    # Write-Host ""
}

$selected = $($appVOs | Out-ConsoleGridView -OutputMode Single)
Write-Host $selected.entry
Write-Host $selected.clsPath
