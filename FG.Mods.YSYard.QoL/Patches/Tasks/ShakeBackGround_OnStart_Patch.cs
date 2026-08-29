using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(ShakeBackGround), nameof(ShakeBackGround.lzd))]
public static class ShakeBackGround_OnStart_Patch
{
    public static void Prefix(ShakeBackGround __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //__instance?.Data?.duration = 0f;
            __instance?.bhlf?.bdyl = 0f;
        }
    }

    public static void Postfix(ShakeBackGround __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
