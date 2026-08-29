using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(CartoonSet), nameof(CartoonSet.lzd))]
public static class CartoonSet_OnStart_Patch
{
    public static void Prefix(CartoonSet __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //__instance?.Data?.fadeIn = false;
            __instance?.bhlf?.bdxg = false;
            //__instance?.Data?.fadeOut = false;
            __instance?.bhlf?.bdxh = false;
        }
    }

    public static void Postfix(CartoonSet __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
