# Domain Personalization Setup

An automated setup tool designed for Windows domain environments, guiding new users through personalizing their workspace.

## Features

- **First Login Detection**: Automatically runs on first login and tracks completion per-user in the registry.
- **Dynamic Branding**: Admin-configurable organization name and logo.
- **Theme Selection**: Choose between Light and Dark modes.
- **Wallpaper Selection**: Easily browse and set a desktop wallpaper.
- **Quick Access**: Simple access to taskbar alignment and default app settings.
- **Admin Controls**: Configure available features and defaults via a `config.json` file.
- **Non-Admin Safe**: Does not require administrative privileges.

## Requirements

- Windows 10 or 11
- .NET 8.0 or newer (built with .NET 10.0)

## Configuration

The application searches for `config.json` in the following locations (in order):

1. Path specified in `HKLM\Software\DomainPersonalizationSetup\ConfigPath`.
2. Same directory as the application executable.
3. `%ProgramData%\DomainPersonalizationSetup\config.json`.

### Example `config.json`

```json
{
  "Branding": {
    "OrganizationName": "Contoso Corp",
    "LogoPath": "C:\\Branding\\logo.png",
    "AccentColor": "#0078D4"
  },
  "Features": {
    "EnableWallpaper": true,
    "EnableTheme": true,
    "EnableTaskbar": true,
    "EnableDefaultApps": true
  }
}
```

## Deployment

The application is designed for enterprise deployment via:

### 1. Group Policy Object (GPO) - Task Scheduler
Create a Task Scheduler GPO that runs `DomainPersonalizationSetup.exe` at user logon.

### 2. GPO - Login Script
Add the executable to a logon script in the user's GPO settings.

### 3. Startup Folder
Copy the application or a shortcut to `%ProgramData%\Microsoft\Windows\Start Menu\Programs\StartUp`.

## Logging

Logs are stored locally at `%AppData%\DomainPersonalizationSetup\logs.txt` for troubleshooting.

## License

Licensed under the [Apache License, Version 2.0](LICENSE).
