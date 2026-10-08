# 🖱️ No Back Key / Menu Fix for Cinderbox

> A lightweight [SMAPI](https://smapi.io/) mod designed specifically for **Cinderbox** (Stardew Valley on Android).

---

## 🐛 The Problem
When using a physical mouse with Cinderbox on Android, releasing the **Right-Click** button can cause the Android OS to send a system **Back** signal, which the game receives as the `Escape` key. This causes in-game menus (such as the Letter Viewer, Inventory, Chests, or Shop menus) to close unexpectedly whenever right-click is released inside them.

## ✨ What this Mod Does
This mod suppresses the `Escape` (back) key along with any user-configured extra buttons, preventing the trailing back command from closing your menus. Right-click itself is **not** blocked and continues to function normally.

---

## 🚀 Features
- **Menu Leak Fix:** Prevents the trailing `Escape` / Back signal from closing open menus.
- **Grace Period Suppression:** Continues swallowing `Escape` inputs for a configurable number of game ticks (`SuppressGraceTicks`) after button release to eliminate phantom or lingering signals.
- **Key-Up Suppression:** Swallows both button-down and button-up events to ensure clean input filtering.
- **Extra Blocked Buttons:** Configure additional buttons to suppress if your device maps the back action to a different key code.
- **Toggle Hotkey:** Press `F9` (customizable) to dynamically enable or disable the mod in-game.
- **Diagnostic Logging:** Optional `DebugLogButtons` option logs every button press to the SMAPI console to identify hardware-specific key codes.
- **Generic Mod Config Menu (GMCM) Integration:** Full in-game configuration UI when GMCM is installed.
- **Multi-language Support:** Ready for internationalization via SMAPI's `i18n` system (English and Thai included).

---

## 🏗️ Architecture & Developer Guide

This section is for developers who want to maintain, fork, or extend this mod.

### Project Structure
```
NoBackKey/
├── NoBackKey.csproj              # .NET 8 SDK project file targeting SMAPI 4.0+
├── manifest.json                 # Mod metadata and unique identifier
├── ModEntry.cs                   # Main SMAPI entry point and event handlers
├── ModConfig.cs                  # Configuration data model
├── IGenericModConfigMenuApi.cs   # Interface definition for GMCM integration
├── README.md                     # Documentation & developer guide
└── i18n/                         # Localization dictionaries
    ├── default.json              # Default strings (English)
    └── th.json                   # Thai translation
```

### Core Logic Overview

1. **`ModEntry.cs`**:
   - Subscribes to `ButtonPressed`, `ButtonReleased`, `UpdateTicked`, and `GameLaunched`.
   - **`OnButtonPressed`**: Checks if the pressed key is the configured `ToggleKey`. If so, flips `Config.Enabled` and shows a HUD notification. If `ShouldBlock(button)` returns true, calls `Helper.Input.Suppress(button)` and resets `graceTicks = Config.SuppressGraceTicks`.
   - **`OnButtonReleased`**: Suppresses the button release event as well to stop delayed key-up events from triggering game actions.
   - **`OnUpdateTicked`**: Decrements `graceTicks` on each frame (60 ticks/sec). While `graceTicks > 0`, it actively calls `Helper.Input.Suppress(SButton.Escape)` to catch delayed OS events.
   - **`OnGameLaunched`**: Looks up `spacechase0.GenericModConfigMenu` in `Helper.ModRegistry` and binds configuration options.

2. **`ModConfig.cs`**:
   - `Enabled` (`bool`): Master toggle for all suppression logic.
   - `BlockEscape` (`bool`): Specifically targets `SButton.Escape`.
   - `ExtraBlockedButtons` (`KeybindList`): Custom list of buttons to suppress.
   - `SuppressGraceTicks` (`int`): Duration in ticks to keep suppressing (default: 4).
   - `ToggleKey` (`KeybindList`): In-game hotkey to toggle mod (default: `F9`).
   - `DebugLogButtons` (`bool`): Diagnostic logging to console.

3. **Input Handling Notes**:
   - SMAPI's `Helper.Input.Suppress(button)` cancels the current input event so the game never receives it.
   - `Helper.Input.SuppressActiveKeybinds(keybind)` prevents the toggle key itself from leaking into game actions.

---

## 🛠️ Building from Source

This project does not use NuGet package references for the game binaries, making it easily portable across custom Android/mobile setups.

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher.
- Stardew Valley game & SMAPI binaries:
  - `StardewModdingAPI.dll`
  - `Stardew Valley.dll`
  - `MonoGame.Framework.dll`
  - `xTile.dll`
  - `smapi-internal/SMAPI.Toolkit.CoreInterfaces.dll`

### Build Steps
1. Open `NoBackKey.csproj` and configure the `<GameDir>` property to point to your directory containing the above DLLs:
   ```xml
   <GameDir>/path/to/your/StardewValley</GameDir>
   ```
2. Build the project in Release configuration:
   ```bash
   dotnet build -c Release
   ```
3. The build output will be placed in `bin/Release/`, and `manifest.json` along with the `i18n` folder will be copied automatically.
4. Deploy the `NoBackKey` folder containing `NoBackKey.dll`, `manifest.json`, and `i18n/` to your Stardew Valley `Mods` folder.

---

## ⚙️ Configuration Reference

Settings can be modified via Generic Mod Config Menu in-game or by editing `config.json` directly:

| Option | Type | Default | Description |
| :--- | :---: | :---: | :--- |
| `Enabled` | `bool` | `true` | Enables or disables the mod entirely. |
| `BlockEscape` | `bool` | `true` | Suppresses the `Escape` key. |
| `ExtraBlockedButtons` | `KeybindList` | `""` | Additional buttons to suppress (comma-separated button names). |
| `ToggleKey` | `KeybindList` | `"F9"` | In-game hotkey to toggle the mod on or off. |
| `SuppressGraceTicks` | `int` | `4` | Extra ticks to keep suppressing `Escape` after release (60 ticks = 1 second). |
| `DebugLogButtons` | `bool` | `false` | Logs all pressed buttons to the SMAPI console for diagnosis. |

---

## 🌐 Localization (i18n)

Translations are stored in `i18n/<locale>.json`.

| Language | File | Status |
| :--- | :--- | :---: |
| **English** | `i18n/default.json` | 🟢 Complete |
| **Thai** | `i18n/th.json` | 🟢 Complete |

To contribute a translation:
1. Copy `i18n/default.json`.
2. Name it using your language code (e.g., `es.json`, `de.json`, `zh.json`, `ja.json`).
3. Translate the values and submit a Pull Request.

---

## 📄 License
This project is licensed under the **MIT License**. You are free to modify, distribute, and contribute.
