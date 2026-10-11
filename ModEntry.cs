using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace NoBackKey
{
    /// <summary>The main entry point for the No Back Key mod.</summary>
    /// <remarks>
    /// Designed specifically for Android environments (e.g. Cinderbox) where physical mice
    /// emit an Android "Back" signal (interpreted as Escape) when releasing the right-click button,
    /// causing in-game menus to prematurely close.
    /// </remarks>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The active mod configuration instance.</summary>
        private ModConfig Config = null!;

        /// <summary>The translation helper for reading localized strings from i18n.</summary>
        private ITranslationHelper I18n = null!;

        /// <summary>Remaining game ticks to continue suppressing the Escape key after a blocked button release.</summary>
        private int graceTicks;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded by SMAPI.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            this.I18n = helper.Translation;

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

            this.Monitor.Log(
                this.I18n.Get("log.loaded", new
                {
                    escape = this.Config.BlockEscape
                }),
                LogLevel.Info
            );
        }

        /*********
        ** Private methods: Input handling
        *********/
        /// <summary>Raised after the player presses a button on any input device (keyboard, mouse, gamepad).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (this.Config.DebugLogButtons)
                this.Monitor.Log(this.I18n.Get("log.pressed", new { button = e.Button }), LogLevel.Info);

            // Toggle mod enable/disable hotkey
            if (this.Config.ToggleKey.JustPressed())
            {
                this.Config.Enabled = !this.Config.Enabled;
                this.Helper.WriteConfig(this.Config);

                Game1.addHUDMessage(new HUDMessage(
                    this.I18n.Get(this.Config.Enabled ? "hud.enabled" : "hud.disabled"), 2));

                this.Helper.Input.SuppressActiveKeybinds(this.Config.ToggleKey);
                return;
            }

            // Suppress unwanted button presses
            if (this.ShouldBlock(e.Button))
            {
                this.Helper.Input.Suppress(e.Button);
                this.graceTicks = this.Config.SuppressGraceTicks;

                if (this.Config.DebugLogButtons)
                    this.Monitor.Log(this.I18n.Get("log.blocked", new { button = e.Button }), LogLevel.Debug);
            }
        }

        /// <summary>Raised after the player releases a button on any input device.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
        {
            // Also suppress the key-up event to prevent phantom or lingering presses upon release
            if (this.ShouldBlock(e.Button))
            {
                this.Helper.Input.Suppress(e.Button);
                this.graceTicks = this.Config.SuppressGraceTicks;
            }
        }

        /// <summary>Raised once per tick during the main game loop update.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (this.graceTicks <= 0)
                return;

            this.graceTicks--;

            // Continue suppressing Escape during grace period
            if (this.Config.BlockEscape)
                this.Helper.Input.Suppress(SButton.Escape);
        }

        /// <summary>Determines whether a given button press or release should be suppressed.</summary>
        /// <param name="button">The button to inspect.</param>
        /// <returns>True if the button should be suppressed; otherwise, false.</returns>
        private bool ShouldBlock(SButton button)
        {
            if (!this.Config.Enabled)
                return false;

            // Suppress Escape key if configured
            if (button == SButton.Escape && this.Config.BlockEscape)
                return true;

            // Suppress user-configured extra buttons
            if (this.IsInKeybindList(this.Config.ExtraBlockedButtons, button))
                return true;

            return false;
        }

        /// <summary>Checks whether a button is part of any binding in the specified KeybindList.</summary>
        /// <param name="list">The keybind list to search within.</param>
        /// <param name="button">The button to find.</param>
        /// <returns>True if the button is found in the list; otherwise, false.</returns>
        private bool IsInKeybindList(KeybindList list, SButton button)
        {
            return list?.Keybinds
                .Any(bind => bind.Buttons.Contains(button))
                ?? false;
        }

        /*********
        ** Private methods: Config menu (GMCM)
        *********/
        /// <summary>Raised after the game is launched, right before the first update tick. Used to integrate with GMCM.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var api = this.Helper.ModRegistry
                .GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api is null)
            {
                this.Monitor.Log(this.I18n.Get("log.gmcm-missing"), LogLevel.Debug);
                return;
            }

            // Register mod with GMCM
            api.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // Introductory description
            api.AddParagraph(
                mod: this.ModManifest,
                text: () => this.I18n.Get("config.intro")
            );

            // --- Main Settings ---
            api.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.I18n.Get("config.section.main")
            );

            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.I18n.Get("config.enabled.name"),
                tooltip: () => this.I18n.Get("config.enabled.tooltip"),
                getValue: () => this.Config.Enabled,
                setValue: v => this.Config.Enabled = v,
                fieldId: "Enabled"
            );

            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.I18n.Get("config.block-escape.name"),
                tooltip: () => this.I18n.Get("config.block-escape.tooltip"),
                getValue: () => this.Config.BlockEscape,
                setValue: v => this.Config.BlockEscape = v,
                fieldId: "BlockEscape"
            );

            // --- Key Bindings ---
            api.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.I18n.Get("config.section.keys")
            );

            api.AddKeybindList(
                mod: this.ModManifest,
                name: () => this.I18n.Get("config.extra-buttons.name"),
                tooltip: () => this.I18n.Get("config.extra-buttons.tooltip"),
                getValue: () => this.Config.ExtraBlockedButtons,
                setValue: v => this.Config.ExtraBlockedButtons = v,
                fieldId: "ExtraBlockedButtons"
            );

            api.AddKeybindList(
                mod: this.ModManifest,
                name: () => this.I18n.Get("config.toggle-key.name"),
                tooltip: () => this.I18n.Get("config.toggle-key.tooltip"),
                getValue: () => this.Config.ToggleKey,
                setValue: v => this.Config.ToggleKey = v,
                fieldId: "ToggleKey"
            );

            // --- Advanced Settings ---
            api.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.I18n.Get("config.section.advanced")
            );

            api.AddNumberOption(
                mod: this.ModManifest,
                name: () => this.I18n.Get("config.grace-ticks.name"),
                tooltip: () => this.I18n.Get("config.grace-ticks.tooltip"),
                getValue: () => this.Config.SuppressGraceTicks,
                setValue: v => this.Config.SuppressGraceTicks = v,
                min: 0,
                max: 60,
                interval: 1,
                fieldId: "SuppressGraceTicks"
            );

            // --- Debug Settings (Developers) ---
            api.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.I18n.Get("config.section.debug")
            );

            api.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.I18n.Get("config.debug-log.name"),
                tooltip: () => this.I18n.Get("config.debug-log.tooltip"),
                getValue: () => this.Config.DebugLogButtons,
                setValue: v => this.Config.DebugLogButtons = v,
                fieldId: "DebugLogButtons"
            );

            api.OnFieldChanged(this.ModManifest, (fieldId, value) =>
            {
                if (this.Config.DebugLogButtons)
                    this.Monitor.Log($"config changed: {fieldId} = {value}", LogLevel.Trace);
            });
        }
    }
}
