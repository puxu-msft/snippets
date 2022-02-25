# wt-gen-unbound-list.ps1

Generate unbound list for Windows Terminal config.json

```json
// Application-level Keys

// {"command":"unbound","keys":"alt+f4"}, // "closeWindow"
{"command":"unbound","keys":"alt+enter"}, // "toggleFullscreen"
// {"command":"unbound","keys":"f11"}, // "toggleFullscreen"
{"command":"unbound","keys":"ctrl+shift+space"}, // "openNewTabDropdown"
{"command":"unbound","keys":"ctrl+,"}, // {"action":"openSettings","target":"settingsUI"}
{"command":"unbound","keys":"ctrl+shift+,"}, // {"action":"openSettings","target":"settingsFile"}
{"command":"unbound","keys":"ctrl+alt+,"}, // {"action":"openSettings","target":"defaultsFile"}
{"command":"unbound","keys":"ctrl+shift+f"}, // "find"
// {"command":"unbound","keys":"ctrl+shift+p"}, // "commandPalette"
{"command":"unbound","keys":"win+sc(41)"}, // "quakeMode"
```
