using HarmonyLib;

namespace Landoria.AutoCloseDoors
{
    // Adds the local shortcut and schedules vanilla door closure.
    [HarmonyPatch]
    internal static class DoorPatches
    {
        // Shows the current choice next to the normal door action.
        [HarmonyPatch(typeof(Door), nameof(Door.GetHoverText))]
        [HarmonyPostfix]
        private static void Hover(Door __instance, ref string __result)
        {
            if (!DoorChoice.IsTarget(__instance) || string.IsNullOrEmpty(__result))
            {
                return;
            }
            string status = DoorChoice.IsEnabled(__instance) ? "on" : "off";
            __result += "\n[<color=yellow><b>Shift + $KEY_Use</b></color>] Auto-close: " + status;
        }

        // Handles Shift+E before Valheim toggles the door.
        [HarmonyPatch(typeof(Door), nameof(Door.Interact))]
        [HarmonyPrefix]
        private static bool Interact(Door __instance, Humanoid character, bool hold,
            bool alt, ref bool __result)
        {
            if (!DoorChoice.IsTarget(__instance) || hold || !alt ||
                character != Player.m_localPlayer)
            {
                return true;
            }
            if (__instance.m_checkGuardStone &&
                !PrivateArea.CheckAccess(__instance.transform.position))
            {
                __result = true;
                return false;
            }
            DoorChoice.Toggle(__instance);
            __result = true;
            return false;
        }

        // Starts a timer only when the local player opens an enabled door.
        [HarmonyPatch(typeof(Door), nameof(Door.Interact))]
        [HarmonyPostfix]
        private static void AfterInteract(Door __instance, Humanoid character,
            bool hold, bool alt, bool __result)
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
            ZNetView view = __instance.GetComponent<ZNetView>();
            if (view == null || !view.IsValid())
            {
                return;
            }
            if (view.GetZDO().GetInt(ZDOVars.s_state) == 0 &&
                DoorChoice.IsEnabled(__instance))
            {
                DoorTimers.Schedule(__instance);
            }
            else
            {
                DoorTimers.Cancel(__instance);
            }
        }
    }
}
