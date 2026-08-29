using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using HotelModule;

namespace FG.Mods.YSYard.QoL.Patches;

[HarmonyPatch(typeof(rm), nameof(rm.hxj))]
public static class HotelAttributes_Init_Patch
{
    public static void Postfix(rm __instance)
    {
        ExHotelAttributes.Init(__instance);
    }
}
