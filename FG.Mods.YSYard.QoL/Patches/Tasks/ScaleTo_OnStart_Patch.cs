using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(ScaleTo), nameof(ScaleTo.lzd))]
public static class ScaleTo_OnStart_Patch
{
    public static void Prefix(ScaleTo __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //__instance?.Data?.duration = 0f;
            __instance?.bhlf?.bdsy = 0f;
        }
    }

    public static void Postfix(ScaleTo __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
