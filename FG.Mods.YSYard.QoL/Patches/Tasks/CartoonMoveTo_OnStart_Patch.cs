using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(CartoonMoveTo), nameof(CartoonMoveTo.lzd))]
public static class CartoonMoveTo_OnStart_Patch
{
    public static void Prefix(CartoonMoveTo __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //__instance?.Data?.duration = 0f;
            __instance?.bhlf?.bdxm = 0f;
        }
    }

    public static void Postfix(CartoonMoveTo __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
