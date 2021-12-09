# start.ps1

Usage: `pwsh ./start.ps1`

Because for current pwsh version, in one pwsh session, the assembly loaded by `Add-Type` cannot be `Remove-Type`.

## Why this?

Win32 API CreateProcess accepts a flag `CREATE_NEW_PROCESS_GROUP`, but both C# `System.Diagnostics.Process` and pwsh `Start-Process` forgets this feature.

But pwsh can load C# assembly by `Add-Type`, and C# can access Win32 API via Native Interop API. So we can make it.
