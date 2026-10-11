using HarmonyLib;

namespace Landoria.AutoCloseDoors
{
    // Adds the local shortcut and tracks opened doors.
    [HarmonyPatch]
    internal static class DoorPatches
    {
        // Shows the current choice next to the normal door action.
        [HarmonyPatch(typeof(Door), nameof(Door.GetHoverText))]
        [HarmonyPostfix]
        private static void Hover(Door __instance, ref string __result)
        {
            if (!DoorChoice.CanClose(__instance) || string.IsNullOrEmpty(__result))
            {
                return;
            }
            string status = DoorChoice.IsEnabled(__instance) ? "on" : "off";
            string shortcut = Localization.instance.Localize("$KEY_Use");
            __result += "\n[<color=yellow><b>Shift + " + shortcut +
                        "</b></color>] Auto-close: " + status;
        }

        // Handles Shift+E before Valheim toggles the door.
        [HarmonyPatch(typeof(Door), nameof(Door.Interact))]
        [HarmonyPrefix]
        private static bool Interact(Door __instance, Humanoid character, bool hold,
            bool alt, ref bool __result, out bool __state)
        {
            __state = false;
            if (DoorChoice.IsTarget(__instance) && !hold && !alt &&
                character == Player.m_localPlayer)
            {
                ZNetView view = __instance.GetComponent<ZNetView>();
                __state = view != null && view.IsValid() &&
                          view.GetZDO().GetInt(ZDOVars.s_state) == 0;
            }
            if (!DoorChoice.IsTarget(__instance) || hold || !alt ||
                character != Player.m_localPlayer)
            {
                return true;
            }
            if (!DoorChoice.CanClose(__instance))
            {
                return true;
            }
            DoorChoice.Toggle(__instance);
            __result = true;
            return false;
        }

        // Tracks a door when the local player opens it.
        [HarmonyPatch(typeof(Door), nameof(Door.Interact))]
        [HarmonyPostfix]
        private static void AfterInteract(Door __instance, Humanoid character,
            bool hold, bool alt, bool __result, bool __state)
        {
            if (!DoorChoice.IsTarget(__instance) || hold || alt || !__result ||
                character != Player.m_localPlayer)
            {
                return;
            }
            if (__instance.m_checkGuardStone &&
                !PrivateArea.CheckAccess(__instance.transform.position, 0f, false))
            {
                return;
            }
            if (__state && DoorChoice.IsEnabled(__instance))
            {
                DoorProximity.Track(__instance);
            }
            else
            {
                DoorProximity.Cancel(__instance);
            }
        }
    }
}
