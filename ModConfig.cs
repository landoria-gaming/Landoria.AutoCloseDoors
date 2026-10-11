using BepInEx.Configuration;

namespace Landoria.AutoCloseDoors
{
    // Owns the mod settings and their default values.
    internal sealed class ModConfig
    {
        private readonly ConfigFile config;
        private readonly ConfigEntry<bool> enabledByDefault;
        private readonly ConfigEntry<float> closeDistance;

        internal bool EnabledByDefault => enabledByDefault.Value;
        internal float CloseDistanceMeters => closeDistance.Value;

        // Binds the settings to the existing BepInEx keys.
        internal ModConfig(ConfigFile configFile)
        {
            config = configFile;
            enabledByDefault = config.Bind("General", "EnabledByDefault", true,
                "Whether supported doors close automatically unless changed with Shift+E.");
            closeDistance = config.Bind("General", "CloseDistanceMeters", 5f,
                "Distance players must leave before an enabled door closes automatically.");
        }

        // Restores defaults after the configuration file is deleted.
        internal void RestoreDefaults()
        {
            enabledByDefault.Value = true;
            closeDistance.Value = 5f;
            config.Save();
            ConfigWatcher.IgnoreCurrentFileVersion();
        }
    }
}
