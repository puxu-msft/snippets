# msvc-symlink.ps1

Usage: `./msvc-symlink.ps1 -d C:\my\local\links\msvc`

## Why this?

The path to MSVC component in VS folder changes with version updates, the old version is even removed.

It's annoying when you just want to use it handly in spite of the versioning.

## Prerequisite

```ps1
Install-Module -Scope CurrentUser Microsoft.PowerShell.ConsoleGuiTools
Install-Module -Scope CurrentUser VSSetup
```
