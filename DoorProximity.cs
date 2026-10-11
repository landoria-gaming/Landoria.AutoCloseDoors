using System.Collections.Generic;
using UnityEngine;

namespace Landoria.AutoCloseDoors
{
    // Closes tracked doors after every nearby player leaves.
    internal static class DoorProximity
    {
        private const float OpenRequestTimeout = 2f;
        private static readonly Dictionary<Door, float> Tracked = new Dictionary<Door, float>();
        private static readonly List<Door> Finished = new List<Door>();

        // Tracks a door opened by the local player.
        internal static void Track(Door door)
        {
            Tracked[door] = Time.time;
        }

        // Stops tracking a door after a manual close or preference change.
        internal static void Cancel(Door door)
        {
            Tracked.Remove(door);
        }

        // Checks tracked doors on the Unity thread.
        internal static void Update()
        {
            Finished.Clear();
            float distance = Mathf.Max(0f, Plugin.Settings.CloseDistanceMeters);
            foreach (KeyValuePair<Door, float> entry in Tracked)
            {
                Door door = entry.Key;
                if (door == null || !DoorChoice.IsEnabled(door))
                {
                    Finished.Add(door);
                    continue;
                }
                ZNetView view = door.GetComponent<ZNetView>();
                if (view == null || !view.IsValid())
                {
                    Finished.Add(door);
                    continue;
                }
                if (view.GetZDO().GetInt(ZDOVars.s_state) == 0)
                {
                    if (Time.time - entry.Value >= OpenRequestTimeout)
                    {
                        Finished.Add(door);
                    }
                    continue;
                }
                if (Player.IsPlayerInRange(door.transform.position, distance))
                {
                    continue;
                }
                if (!DoorChoice.CanClose(door))
                {
                    continue;
                }
                view.InvokeRPC("UseDoor", false);
                Finished.Add(door);
            }
            foreach (Door door in Finished)
            {
                Tracked.Remove(door);
            }
        }

        // Drops tracked doors when the plugin unloads.
        internal static void Clear()
        {
            Tracked.Clear();
            Finished.Clear();
        }
    }
}
