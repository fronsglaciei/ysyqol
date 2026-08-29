using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(MovePath), nameof(MovePath.lzd))]
public static class MovePath_OnStart_Patch
{
    public static void Prefix(MovePath __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //__instance?.Data?.duration = 0.001f;
            __instance?.bhlf?.bdoa = 0.001f;
        }
    }

    public static void Postfix(MovePath __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
