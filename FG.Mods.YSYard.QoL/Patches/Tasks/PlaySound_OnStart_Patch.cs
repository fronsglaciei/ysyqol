using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(PlaySound), nameof(PlaySound.lzd))]
public static class PlaySound_OnStart_Patch
{
    public static void Postfix(PlaySound __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
