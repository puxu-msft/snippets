param(
    [switch]$Detach,

    [Parameter(ValueFromRemainingArguments)]
    [string[]]$Remaining
)

$exe = .\a.exe

Push-Location $PSScriptRoot
try {
    Start-Process $exe -ArgumentList @Arguments `
        -Wait:$(-not $Detach) -NoNewWindow:$(-not $Detach)
}
finally {
    Pop-Location
}
