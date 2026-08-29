using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(bfi), nameof(bfi.cnm))]
public static class GameSettingWindow_OnInit_Patch
{
    public static void Postfix(bfi __instance)
    {
        ExUIManager.CreateModSettingPanel(__instance);
    }
}
