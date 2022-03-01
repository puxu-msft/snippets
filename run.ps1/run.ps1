param(
    [Parameter(Mandatory, Position=0)]
    [string]$Exe,

    [switch]$Detach,

    [Parameter(ValueFromRemainingArguments)]
    [string[]]$Remaining
)

# Push-Location $PSScriptRoot
[Console]::TreatControlCAsInput = $true
try {
    Start-Process $Exe -ArgumentList $Remaining `
        -Wait:$(-not $Detach) -NoNewWindow:$(-not $Detach)
}
finally {
    [Console]::TreatControlCAsInput = $false
    # Pop-Location
}
