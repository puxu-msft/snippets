# list-registered-apps.ps1

Usage: `pwsh ./get-registered-apps.ps1`

Because for current pwsh version, in one pwsh session, the assembly loaded by `Add-Type` cannot be `Remove-Type`.

This pwsh script includes:

- Access Windows Registry
- Use C# interop API
- Parse indirect string
