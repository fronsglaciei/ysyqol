using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(MoveTo), nameof(MoveTo.lzd))]
public static class MoveTo_OnStart_Patch
{
    public static void Prefix(MoveTo __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //__instance?.Data?.duration = 0f;
            __instance?.bhlf?.bdoj = 0f;
        }
    }

    public static void Postfix(MoveTo __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
