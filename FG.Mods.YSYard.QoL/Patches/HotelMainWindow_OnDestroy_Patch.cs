using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(HotelMainWindow), nameof(HotelMainWindow.cnw))]
public static class HotelMainWindow_OnDestroy_Patch
{
    public static void Postfix()
    {
        ExUIManager.OnHotelMainWindowDestroyed();
    }
}
