using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(PlayBGM), nameof(PlayBGM.lzd))]
public static class PlayBGM_OnStart_Patch
{
    public static void Prefix(PlayBGM __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //var duration = __instance?.Data?.duration;
            var duration = __instance?.bhlf?.bdjp;
            if (duration is null || duration == 0f)
            {
                return;
            }

            //__instance.Data.duration = 0.01f;
            __instance.bhlf.bdjp = 0.01f;
        }
    }

    public static void Postfix(PlayBGM __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
