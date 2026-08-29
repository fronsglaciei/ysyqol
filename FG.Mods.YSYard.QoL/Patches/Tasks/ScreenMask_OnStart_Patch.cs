using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(ScreenMask), nameof(ScreenMask.lzd))]
public static class ScreenMask_OnStart_Patch
{
    public static void Prefix(ScreenMask __instance)
    {
        //__instance?.Data?.fadeDuration = 0f;
        __instance?.bhlf?.bdtt = 0f;
    }

    public static void Postfix(ScreenMask __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
