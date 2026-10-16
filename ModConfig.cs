using BepInEx.Configuration;
using UnityEngine;

namespace Landoria.AutoCloseDoors
{
    internal enum ToggleModifierOption
    {
        LeftAlt,
        LeftShift,
        LeftControl
    }

    // Owns the mod settings and their default values.
    internal sealed class ModConfig
    {
        private readonly ConfigFile config;
        private readonly ConfigEntry<bool> enabledByDefault;
        private readonly ConfigEntry<float> closeDistance;
        private readonly ConfigEntry<ToggleModifierOption> toggleModifier;

        internal bool EnabledByDefault => enabledByDefault.Value;
        internal float CloseDistanceMeters => closeDistance.Value;
        internal KeyCode ToggleModifier => toggleModifier.Value == ToggleModifierOption.LeftShift
            ? KeyCode.LeftShift : toggleModifier.Value == ToggleModifierOption.LeftControl
                ? KeyCode.LeftControl : KeyCode.LeftAlt;

        internal string ToggleModifierName => toggleModifier.Value == ToggleModifierOption.LeftShift
            ? "Shift" : toggleModifier.Value == ToggleModifierOption.LeftControl
                ? "Ctrl" : "Alt";

        // Binds the settings to the existing BepInEx keys.
        internal ModConfig(ConfigFile configFile)
        {
            config = configFile;
            enabledByDefault = config.Bind("General", "EnabledByDefault", true,
                "Whether supported doors close automatically unless changed with the modifier and use key.");
            closeDistance = config.Bind("General", "CloseDistanceMeters", 5f,
                "Distance players must leave before an enabled door closes automatically.");
            toggleModifier = config.Bind("General", "ToggleModifier", ToggleModifierOption.LeftAlt,
                "Modifier used with the door interaction key to toggle auto-close.");
        }

        // Restores defaults after the configuration file is deleted.
        internal void RestoreDefaults()
        {
            enabledByDefault.Value = true;
            closeDistance.Value = 5f;
            toggleModifier.Value = ToggleModifierOption.LeftAlt;
            config.Save();
            ConfigWatcher.IgnoreCurrentFileVersion();
        }
    }
}
