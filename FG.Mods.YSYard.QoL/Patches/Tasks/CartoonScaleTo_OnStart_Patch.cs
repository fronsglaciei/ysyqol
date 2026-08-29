using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(CartoonScaleTo), nameof(CartoonScaleTo.lzd))]
public static class CartoonScaleTo_OnStart_Patch
{
    public static void Prefix(CartoonScaleTo __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //__instance?.Data?.duration = 0f;
            __instance?.bhlf?.bdxv = 0f;
        }
    }

    public static void Postfix(CartoonScaleTo __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
