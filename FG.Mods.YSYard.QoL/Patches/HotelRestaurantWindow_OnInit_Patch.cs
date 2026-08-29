using FG.Mods.YSYard.QoL.Services;
using Foundation.UI;
using HarmonyLib;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(bcx), nameof(bcx.cnm))]
public static class HotelRestaurantWindow_OnInit_Patch
{
    public static void Postfix(bcx __instance)
    {
        ExUIManager.OnHotelRestaurantWindowInit(__instance);
    }
}
