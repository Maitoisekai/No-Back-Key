using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace NoBackKey
{
    /// <summary>The API provided by Generic Mod Config Menu (spacechase0.GenericModConfigMenu).</summary>
    public interface IGenericModConfigMenuApi
    {
        /*********
        ** Register
        *********/
        /// <summary>Register a mod whose configuration can be edited in the menu.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="reset">Reset the mod's config to its default values.</param>
        /// <param name="save">Save the mod's current config to disk.</param>
        /// <param name="titleScreenOnly">Whether the options can only be edited from the title screen.</param>
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        /// <summary>Remove a mod from the config menu and clear all registered options.</summary>
        /// <param name="mod">The mod's manifest.</param>
        void Unregister(IManifest mod);

        /*********
        ** Basic options
        *********/
        /// <summary>Add a section title at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="text">The title text to display.</param>
        /// <param name="tooltip">The tooltip text shown when hovering over the title, if any.</param>
        void AddSectionTitle(IManifest mod, Func<string> text, Func<string>? tooltip = null);

        /// <summary>Add a paragraph of descriptive text at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="text">The paragraph text to display.</param>
        void AddParagraph(IManifest mod, Func<string> text);

        /// <summary>Add a boolean checkbox option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to display.</param>
        /// <param name="tooltip">The tooltip text shown when hovering over the option, if any.</param>
        /// <param name="fieldId">The unique field ID for change events.</param>
        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue,
            Func<string> name, Func<string>? tooltip = null, string? fieldId = null);

        /// <summary>Add an integer numeric option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to display.</param>
        /// <param name="tooltip">The tooltip text shown when hovering over the option, if any.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <param name="interval">The step interval between values.</param>
        /// <param name="formatValue">Format the numeric value for display, if custom formatting is needed.</param>
        /// <param name="fieldId">The unique field ID for change events.</param>
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue,
            Func<string> name, Func<string>? tooltip = null,
            int? min = null, int? max = null, int? interval = null,
            Func<int, string>? formatValue = null, string? fieldId = null);

        /// <summary>Add a float numeric option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to display.</param>
        /// <param name="tooltip">The tooltip text shown when hovering over the option, if any.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <param name="interval">The step interval between values.</param>
        /// <param name="formatValue">Format the numeric value for display, if custom formatting is needed.</param>
        /// <param name="fieldId">The unique field ID for change events.</param>
        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue,
            Func<string> name, Func<string>? tooltip = null,
            float? min = null, float? max = null, float? interval = null,
            Func<float, string>? formatValue = null, string? fieldId = null);

        /// <summary>Add a string text option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to display.</param>
        /// <param name="tooltip">The tooltip text shown when hovering over the option, if any.</param>
        /// <param name="allowedValues">The list of allowed string values, if restricted to specific choices.</param>
        /// <param name="formatAllowedValue">Format an allowed value for display.</param>
        /// <param name="fieldId">The unique field ID for change events.</param>
        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue,
            Func<string> name, Func<string>? tooltip = null,
            string[]? allowedValues = null, Func<string, string>? formatAllowedValue = null,
            string? fieldId = null);

        /*********
        ** Keybinds
        *********/
        /// <summary>Add a single key binding option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to display.</param>
        /// <param name="tooltip">The tooltip text shown when hovering over the option, if any.</param>
        /// <param name="fieldId">The unique field ID for change events.</param>
        void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue,
            Func<string> name, Func<string>? tooltip = null, string? fieldId = null);

        /// <summary>Add a multi-key binding list option at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="getValue">Get the current value from the mod config.</param>
        /// <param name="setValue">Set a new value in the mod config.</param>
        /// <param name="name">The label text to display.</param>
        /// <param name="tooltip">The tooltip text shown when hovering over the option, if any.</param>
        /// <param name="fieldId">The unique field ID for change events.</param>
        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue,
            Func<string> name, Func<string>? tooltip = null, string? fieldId = null);

        /*********
        ** Pages & layout
        *********/
        /// <summary>Add a new sub-page at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="pageId">The unique identifier for the sub-page.</param>
        /// <param name="pageTitle">The title text displayed at the top of the sub-page.</param>
        void AddPage(IManifest mod, string pageId, Func<string>? pageTitle = null);

        /// <summary>Add a link to a sub-page at the current position in the form.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="pageId">The identifier of the target sub-page.</param>
        /// <param name="text">The link text to display.</param>
        /// <param name="tooltip">The tooltip text shown when hovering over the link, if any.</param>
        void AddPageLink(IManifest mod, string pageId, Func<string> text, Func<string>? tooltip = null);

        /*********
        ** Events
        *********/
        /// <summary>Register a callback invoked whenever any registered configuration field changes.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="onChange">Callback invoked with (fieldId, newValue).</param>
        void OnFieldChanged(IManifest mod, Action<string, object> onChange);

        /// <summary>Set whether subsequent options added to the form should be editable only from the title screen.</summary>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="titleScreenOnly">Whether subsequent options can only be changed on the title screen.</param>
        void SetTitleScreenOnlyForNextOptions(IManifest mod, bool titleScreenOnly);
    }
}
