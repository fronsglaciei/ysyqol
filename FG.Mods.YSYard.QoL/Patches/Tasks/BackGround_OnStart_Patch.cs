using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(BackGround), nameof(BackGround.lzd))]
public static class BackGround_OnStart_Patch
{
    public static void Prefix(BackGround __instance)
    {
        // PlotWindow.SetFadeBGByType throws exception
        // because PlotWindow.mLastBG and PlotWindow.mLastBGRect are not assigned

        // the code below skips calling PlotWindow.SetFadeBGByType
        // in PlotWindow.BackGroundFadeIn

        //__instance.Data.duration = 0f;
        __instance.bhlf.bdki = 0f;
    }

    public static void Postfix(BackGround __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
