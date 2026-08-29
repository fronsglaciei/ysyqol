using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using HotelModule.UI;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(va), nameof(va.cnm))]
public static class ArtifactSelectWindow_OnInit_Patch
{
    public static void Postfix(va __instance)
    {
        ExUIManager.OnArtifactSelectWindowInit(__instance);
    }
}
