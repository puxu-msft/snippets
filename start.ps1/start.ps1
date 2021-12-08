param(
    [string]$dir,
    [string]$exe,

    # Use Event API
    [int]$waitInSec,

    # Make param() happy
    [switch]$dummy
)

$ErrorActionPreference = 'Stop'

$dir = Resolve-Path $dir

$csFile = Join-Path $PSScriptRoot ".\winapi.cs"
Add-Type -TypeDefinition $(Get-Content $csFile -Raw) | Out-Null

Push-Location $dir
try {
    $exe = Resolve-Path $exe
    $olsArgs = @("`"$exe`"", $config, "/console")

    if ($waitInSec -gt 0) {
        # format: {57c41865-d375-4047-bd76-ecd213363210}
        $uuid = "{{{0}}}" -f $(New-Guid).ToString()
        $script:waitEventHandle = [My.Spec]::CreateEvent($uuid)
        $olsArgs += @("/event", $uuid)
    }

    $procinfo = [My.WinApi+PROCESS_INFORMATION]::new();

    $exeArgs = "{0}" -f [string]::Join(" ", $olsArgs)
    Write-Host "start $exeArgs"
    $ret = [My.Spec]::CreateProcess($exeArgs, [ref]$procinfo);
    if (-not $ret) {
        $err = [System.Runtime.InteropServices.Marshal]::GetLastWin32Error();
        Write-Error "Failed to CreateProcess, err=$err"
        exit 1
    }

    if ($waitInSec -gt 0) {
        $ret = [My.Spec]::WaitFor($procinfo.hProcess, $script:waitEventHandle, 1000 * $waitInSec);
        if (0 -ne $ret) {
            Write-Error "Failed to WaitFor, ret=$("{0:X8}" -f $ret)"
            exit 1
        }
    }
    Write-Host "started"
}
finally {
    Pop-Location
}

Start-Sleep -Seconds 1 | Out-Null
