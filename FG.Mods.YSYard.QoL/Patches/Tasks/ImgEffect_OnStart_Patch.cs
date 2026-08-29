using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(ImgEffect), nameof(ImgEffect.lzd))]
public static class ImgEffect_OnStart_Patch
{
    public static void Postfix(ImgEffect __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
