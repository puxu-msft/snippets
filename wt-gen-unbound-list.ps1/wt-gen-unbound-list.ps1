param(
    [string]$Path,
    [switch]$User,
    [switch]$Default,
    [switch]$Use_V1_4_Key_keybindings
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrEmpty($Path)) {
    if ($User) {
        # Current User Settings
        $packageFamilyName = & powershell -Command { $(Get-AppxPackage -Name Microsoft.WindowsTerminal).PackageFamilyName }
        $Path = "${env:LOCALAPPDATA}\Packages\$packageFamilyName\LocalState\settings.json"
    }

    if ($Default) {
        # Default Settings
        $installLocation = & powershell -Command { $(Get-AppxPackage -Name Microsoft.WindowsTerminal).InstallLocation }
        $Path = "$installLocation\defaults.json"
    }
}

if ([string]::IsNullOrEmpty($Path)) {
    Write-Error 'Input one of -Path <path>, -User, -Default'
    exit(1)
}

$root = Get-Content -Path $Path | ConvertFrom-Json

if ($Use_V1_4_Key_keybindings) {
    $actions = $root.keybindings
}
else {
    $actions = $root.actions
}

# Write-Host $actions
foreach ($action in $actions) {
    # Write-Host $action
    if (-not $action.keys) {
        continue
    }
    $unbound = $action | ConvertTo-Json | ConvertFrom-Json
    $unbound.command = "unbound"

    Write-Host "$($unbound | ConvertTo-Json -Compress), // $($action.command | ConvertTo-Json -Compress)"
}
