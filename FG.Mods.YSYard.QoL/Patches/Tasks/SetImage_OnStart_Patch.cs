using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(SetImage), nameof(SetImage.lzd))]
public static class SetImage_OnStart_Patch
{
    public static void Prefix(SetImage __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //__instance?.Data?.fadeDuration = 0f;
            __instance?.bhlf?.bdua = 0f;
            //__instance?.Data?.changeDuration = 0f;
            __instance?.bhlf?.bduk = 0f;
        }
    }

    public static void Postfix(SetImage __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
