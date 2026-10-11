using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace Landoria.AutoCloseDoors
{
    // Owns the local door behavior and configuration lifecycle.
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "Landoria.AutoCloseDoors";
        public const string PluginName = "AutoClose Doors";
        public const string PluginVersion = "1.0.0";
        internal static Plugin Instance;
        internal ConfigEntry<bool> EnabledByDefault;
        internal ConfigEntry<float> Delay;
        private Harmony harmony;

        // Starts patches and watches the configuration file.
        private void Awake()
        {
            Instance = this;
            EnabledByDefault = Config.Bind("General", "EnabledByDefault", true,
                "Whether supported doors close automatically unless changed with Shift+E.");
            Delay = Config.Bind("General", "CloseDelaySeconds", 5f,
                "Seconds after opening before an enabled door closes automatically.");
            harmony = new Harmony(PluginGuid);
            harmony.PatchAll();
            ConfigWatcher.Initialize(Config, Logger, PluginName, RestoreDefaults);
        }

        // Applies pending configuration reloads and door timers.
        private void Update()
        {
            ConfigWatcher.Update();
            DoorTimers.Update();
        }

        // Restores the default setting when the file is deleted.
        private void RestoreDefaults()
        {
            EnabledByDefault.Value = true;
            Delay.Value = 5f;
            Config.Save();
            ConfigWatcher.IgnoreCurrentFileVersion();
        }

        // Releases patches and the file watcher.
        private void OnDestroy()
        {
            ConfigWatcher.Dispose();
            harmony?.UnpatchSelf();
            DoorTimers.Clear();
            Instance = null;
        }
    }
}
