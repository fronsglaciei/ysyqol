using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(SwitchTransition), nameof(SwitchTransition.lzd))]
public static class SwitchTransition_OnStart_Patch
{
    public static void Prefix(SwitchTransition __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //var stData = __instance?.Data;
            var stData = __instance?.bhlf;
            if (stData is null)
            {
                return;
            }

            //stData.fadeInTime = 0f;
            stData.bdyy = 0f;
            //stData.keepFade = 0f;
            stData.bdyz = 0f;
            //stData.fadeOut = 0f;
            stData.bdza = 0f;
        }
    }

    public static void Postfix(SwitchTransition __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
