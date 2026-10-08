# 🖱️ No Back Key / Menu Fix for Cinderbox

> A lightweight [SMAPI](https://smapi.io/) mod designed specifically for **Cinderbox** (Stardew Valley on Android).

---

## 🐛 The Problem
When using a mouse with Cinderbox on Android, releasing the **"Right-Click"** button can also send the Android system's **"Back"** command, which the game receives as the `Escape` key. This causes game menus (like the Letter Viewer, Inventory, or Shop menus) to close unexpectedly whenever you right-click inside them.

## ✨ What this Mod Does
This mod suppresses the `Escape` (back) key, plus any extra buttons you choose, so the back command that follows a right-click no longer closes your menus. Right-click itself is **not** blocked and keeps working normally.

## 🚀 Features
- **Menu Leak Fix:** Stops the back/`Escape` signal that follows a right-click from closing menus.
- **Extra Blocked Buttons:** Add any other button you want suppressed (useful if your device maps back to something else).
- **Toggle Key:** Press `F9` (default) to turn the mod on or off while playing.
- **Grace Ticks:** Keeps swallowing `Escape` for a few ticks after a blocked press to stop repeated or lingering signals.
- **Debug Logging:** Optionally logs every button press to the SMAPI console to see what is being sent.
- **Config Menu:** Full in-game settings through [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) (optional).
- **Lightweight:** Minimal impact on game performance.

> **Note:** `Escape` is blocked at all times while the mod is enabled, so you cannot open the game menu with it. Close menus with the on-screen X button, or press the toggle key to disable the mod temporarily.

## 📥 Installation
1. Ensure you have [SMAPI for Cinderbox](https://github.com/Ekyso/Cinderbox) installed.
2. Download the latest release from the [Releases page](#).
3. Extract the downloaded folder into your `Mods` directory.
4. Run the game. 🚜

## ⚙️ Configuration
The mod works **out-of-the-box**. To change settings, use Generic Mod Config Menu in-game, or edit `config.json` (created after the first launch).

| Option | Default | Description |
| :--- | :---: | :--- |
| `Enabled` | `true` | Turn the whole mod on or off. |
| `BlockEscape` | `true` | Suppress the `Escape` key. |
| `ExtraBlockedButtons` | empty | Additional buttons to suppress. |
| `ToggleKey` | `F9` | Enable/disable the mod while playing. |
| `SuppressGraceTicks` | `4` | Extra ticks to keep suppressing `Escape` after a blocked press (60 ticks = 1 second). |
| `DebugLogButtons` | `false` | Log every button press to the SMAPI console. |

If menus still close, raise `SuppressGraceTicks`. If you are not sure what your device sends, turn on `DebugLogButtons` and check the SMAPI log.

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
   - **`OnButtonPressed`**: Checks if the pressed key is `ToggleKey`. If so, flips `Config.Enabled` and displays a HUD message. If `ShouldBlock(button)` is true, calls `Helper.Input.Suppress(button)` and sets `graceTicks = Config.SuppressGraceTicks`.
   - **`OnButtonReleased`**: Suppresses button-up as well to stop phantom/delayed inputs from firing.
   - **`OnUpdateTicked`**: Decrements `graceTicks` on each frame (60 ticks/sec). While `graceTicks > 0`, it actively calls `Helper.Input.Suppress(SButton.Escape)` to swallow lingering OS events.
   - **`OnGameLaunched`**: Integrates with `spacechase0.GenericModConfigMenu` if installed.

2. **`ModConfig.cs`**:
   - Holds configuration properties: `Enabled`, `BlockEscape`, `ExtraBlockedButtons`, `SuppressGraceTicks`, `ToggleKey`, and `DebugLogButtons`.

3. **Input Handling Notes**:
   - `Helper.Input.Suppress(button)` intercepts and cancels the button event before game menus receive it.
   - `Helper.Input.SuppressActiveKeybinds(keybind)` prevents the toggle hotkey itself from triggering gameplay actions.

## 🛠️ Building from Source
This project does **not** use `Pathoschild.Stardew.ModBuildConfig`, so there is no automatic game-path detection, deploy, or zip. The game assemblies are referenced directly from a folder you set yourself.

**Requirements**
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A game folder containing the desktop game and SMAPI files:
  - `StardewModdingAPI.dll`
  - `Stardew Valley.dll`
  - `MonoGame.Framework.dll`
  - `xTile.dll`
  - `smapi-internal/SMAPI.Toolkit.CoreInterfaces.dll`

**Steps**
1. Open `NoBackKey.csproj` and set `GameDir` to your game folder:
   ```xml
   <GameDir>/storage/emulated/0/Download/StardewValley</GameDir>
   ```
2. Build:
   ```bash
   dotnet build -c Release
   ```
3. The output goes to `bin/Release/`. `manifest.json` and the `i18n` folder are copied there automatically.
4. Copy `NoBackKey.dll`, `manifest.json`, and the `i18n` folder into a `NoBackKey` folder inside your `Mods` directory.

Because the references are set to `Private=false`, the game DLLs are not copied into the output, and nothing is deployed for you. Copy the files manually or with your own script.

## 📱 Compatibility
- **Cinderbox:** Fully supported. ✅
- **Other Android Ports:** Might work, but primarily designed and tested for Cinderbox. ⚠️

## 🌐 Translations

| Language | Status | Credit |
| :--- | :---: | :--- |
| **English** | 🟢 Complete | — |
| **ไทย (Thai)** | 🟢 Complete | Maito |

**Want to add your language?** 
Copy `i18n/default.json`, rename it to your locale code (e.g., `fr.json`, `de.json`, `zh.json`), translate the values, and open a pull request!

## 🤝 Credits
- **Development Assistance:** The code and documentation for this mod were generated and structured with the assistance of AI tools.

## 📄 License
This project is licensed under the **MIT License**. Feel free to use, modify, and distribute.

---
*Made with ❤️ for the Stardew Valley Android community.*
