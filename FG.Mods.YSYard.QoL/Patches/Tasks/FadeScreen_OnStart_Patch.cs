using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(FadeScreen), nameof(FadeScreen.lzd))]
public static class FadeScreen_OnStart_Patch
{
    public static void Prefix(FadeScreen __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //__instance?.Data?.duration = 0f;
            __instance?.bhlf?.bdmi = 0f;
        }
    }

    public static void Postfix(FadeScreen __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
