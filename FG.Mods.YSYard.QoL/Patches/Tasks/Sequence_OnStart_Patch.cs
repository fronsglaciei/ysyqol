using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(Sequence), nameof(Sequence.lzd))]
public static class Sequence_OnStart_Patch
{
    public static void Postfix(Sequence __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
