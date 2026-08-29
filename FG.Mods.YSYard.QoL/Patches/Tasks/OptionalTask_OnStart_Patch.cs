using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(OptionalTask), nameof(OptionalTask.lzd))]
public static class OptionalTask_OnStart_Patch
{
    public static void Postfix(OptionalTask __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
