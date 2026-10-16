using BepInEx;
using HarmonyLib;

namespace Landoria.AutoCloseDoors
{
    // Owns the local door behavior and configuration lifecycle.
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "Landoria.AutoCloseDoors";
        public const string PluginName = "AutoClose Doors";
        public const string PluginVersion = "1.0.2";
        internal static ModConfig Settings;
        private Harmony harmony;

        // Starts patches and watches the configuration file.
        private void Awake()
        {
            Settings = new ModConfig(Config);
            harmony = new Harmony(PluginGuid);
            harmony.PatchAll();
            ConfigWatcher.Initialize(Config, Logger, PluginName, Settings.RestoreDefaults);
        }

        // Applies pending configuration reloads and checks open doors.
        private void Update()
        {
            ConfigWatcher.Update();
            DoorProximity.Update();
        }

        // Releases patches and the file watcher.
        private void OnDestroy()
        {
            ConfigWatcher.Dispose();
            harmony?.UnpatchSelf();
            DoorProximity.Clear();
            Settings = null;
        }
    }
}
