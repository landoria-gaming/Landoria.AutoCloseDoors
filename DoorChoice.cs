using UnityEngine;

namespace Landoria.AutoCloseDoors
{
    // Stores each character's door overrides in saved character data.
    internal static class DoorChoice
    {
        // Recognizes the three supported buildable doors.
        internal static bool IsTarget(Door door)
        {
            string name = Utils.GetPrefabName(door.gameObject);
            return name == "wood_door" || name == "wood_gate" ||
                   name == "darkwood_gate";
        }

        // Checks whether this character enabled auto-close for the door.
        internal static bool IsEnabled(Door door)
        {
            Player player = Player.m_localPlayer;
            string enabledKey = Key(door, "Enabled");
            string disabledKey = Key(door, "Disabled");
            if (player != null && enabledKey != null)
            {
                if (player.m_customData.ContainsKey(enabledKey))
                {
                    return true;
                }
                if (player.m_customData.ContainsKey(disabledKey))
                {
                    return false;
                }
            }
            return Plugin.Instance?.EnabledByDefault.Value ?? true;
        }

        // Switches the saved choice for the local character.
        internal static void Toggle(Door door)
        {
            Player player = Player.m_localPlayer;
            string enabledKey = Key(door, "Enabled");
            string disabledKey = Key(door, "Disabled");
            if (player == null || enabledKey == null)
            {
                return;
            }
            bool enable = !IsEnabled(door);
            player.m_customData.Remove(enabledKey);
            player.m_customData.Remove(disabledKey);
            player.m_customData[enable ? enabledKey : disabledKey] = "1";
            if (!enable)
            {
                DoorTimers.Cancel(door);
            }
            string status = enable ? "enabled" : "disabled";
            player.Message(MessageHud.MessageType.Center, "Auto-close " + status);
        }

        // Builds a world-specific key from the network object's stable ID.
        private static string Key(Door door, string choice)
        {
            ZNetView view = door.GetComponent<ZNetView>();
            if (view == null || !view.IsValid() || ZNet.instance == null)
            {
                return null;
            }
            ZDOID uid = view.GetZDO().m_uid;
            return "Landoria.AutoCloseDoors." + choice + ":" + ZNet.instance.GetWorldUID() +
                   ":" + uid.UserID + ":" + uid.ID;
        }
    }
}
