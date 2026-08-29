using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using HotelModule.UI;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(
    typeof(vc),
    nameof(vc.cnm))]
public static class RelicRecoveryWindow_OnInit_Patch
{
    public static void Postfix(vc __instance)
    {
        ExUIManager.OnRelicRecoveryWindowInit(__instance);
    }
}
