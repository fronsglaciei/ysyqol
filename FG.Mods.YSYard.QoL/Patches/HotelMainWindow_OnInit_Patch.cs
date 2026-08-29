using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(HotelMainWindow), nameof(HotelMainWindow.cnm))]
public static class HotelMainWindow_OnInit_Patch
{
    public static void Postfix(HotelMainWindow __instance)
    {
        ExUIManager.OnHotelMainWindowInit(__instance);
    }
}
