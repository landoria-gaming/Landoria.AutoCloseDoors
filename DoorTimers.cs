using System.Collections.Generic;
using UnityEngine;

namespace Landoria.AutoCloseDoors
{
    // Closes locally opened doors through Valheim's normal network RPC.
    internal static class DoorTimers
    {
        private static readonly Dictionary<Door, float> OpenedAt =
            new Dictionary<Door, float>();
        private static readonly List<Door> Finished = new List<Door>();

        // Records when the local player opened a door.
        internal static void Schedule(Door door)
        {
            OpenedAt[door] = Time.time;
        }

        // Removes a timer after a manual close or preference change.
        internal static void Cancel(Door door)
        {
            OpenedAt.Remove(door);
        }

        // Handles expired timers on the Unity thread.
        internal static void Update()
        {
            Finished.Clear();
            float delay = Mathf.Max(0f, Plugin.Instance.Delay.Value);
            foreach (KeyValuePair<Door, float> timer in OpenedAt)
            {
                if (timer.Key == null || !DoorChoice.IsEnabled(timer.Key))
                {
                    Finished.Add(timer.Key);
                    continue;
                }
                if (Time.time - timer.Value < delay)
                {
                    continue;
                }
                CloseIfOpen(timer.Key);
                Finished.Add(timer.Key);
            }
            foreach (Door door in Finished)
            {
                OpenedAt.Remove(door);
            }
        }

        // Sends the same owner-directed request as a normal door interaction.
        private static void CloseIfOpen(Door door)
        {
            ZNetView view = door.GetComponent<ZNetView>();
            if (view != null && view.IsValid() &&
                view.GetZDO().GetInt(ZDOVars.s_state) != 0)
            {
                view.InvokeRPC("UseDoor", false);
            }
        }

        // Drops all timers when the plugin unloads.
        internal static void Clear()
        {
            OpenedAt.Clear();
            Finished.Clear();
        }
    }
}
