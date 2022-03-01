# run.ps1

A handy script to run cli program.

## Usage

```pwsh
./run.ps1 cmd.exe /c mklink /?
./run.ps1 filebrowser.exe -Detach
```

## Why this?

- `-Detach` to detach from the current pwsh session.
- Passing `Ctrl+C` to program rather than stopping the script.
