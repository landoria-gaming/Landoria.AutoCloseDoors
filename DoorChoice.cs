using UnityEngine;

namespace Landoria.AutoCloseDoors
{
    // Stores each character's door overrides in saved character data.
    internal static class DoorChoice
    {
        // Recognizes player-built doors that Valheim allows players to close with E.
        internal static bool IsTarget(Door door)
        {
            if (door == null || door.m_keyItem != null || door.m_canNotBeClosed)
            {
                return false;
            }

            Piece piece = door.GetComponent<Piece>();
            return piece != null && piece.IsPlacedByPlayer();
        }

        // Checks the local player's access before closing a door.
        internal static bool CanClose(Door door)
        {
            return IsTarget(door) && Player.m_localPlayer != null &&
                   (!door.m_checkGuardStone ||
                    PrivateArea.CheckAccess(door.transform.position, 0f, false));
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
            return Plugin.Settings?.EnabledByDefault ?? true;
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
            if (enable)
            {
                ZNetView view = door.GetComponent<ZNetView>();
                if (view != null && view.IsValid() &&
                    view.GetZDO().GetInt(ZDOVars.s_state) != 0)
                {
                    DoorProximity.Track(door);
                }
            }
            else
            {
                DoorProximity.Cancel(door);
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
