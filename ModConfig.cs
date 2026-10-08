using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace NoBackKey
{
    /// <summary>The mod configuration model loaded from config.json.</summary>
    public sealed class ModConfig
    {
        /// <summary>Whether the mod functionality is enabled.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Whether to suppress the Escape key entirely.</summary>
        public bool BlockEscape { get; set; } = true;

        /// <summary>Additional user-defined buttons to suppress.</summary>
        public KeybindList ExtraBlockedButtons { get; set; } = new();

        /// <summary>Number of game loop ticks to continue swallowing input after a button press/release (60 ticks = 1 second).</summary>
        public int SuppressGraceTicks { get; set; } = 4;

        /// <summary>Hotkey to toggle the mod enabled state during gameplay.</summary>
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("F9");

        /// <summary>Whether to log every button press to the SMAPI console for diagnostic/troubleshooting purposes.</summary>
        public bool DebugLogButtons { get; set; } = false;
    }
}
