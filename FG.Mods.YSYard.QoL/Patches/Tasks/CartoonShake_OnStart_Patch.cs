using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(CartoonShake), nameof(CartoonShake.lzd))]
public static class CartoonShake_OnStart_Patch
{
    public static void Prefix(CartoonShake __instance)
    {
        if (ConfigProvider.ForceInstantText.Value)
        {
            //__instance?.Data?.duration = 0f;
            __instance?.bhlf?.bdxr = 0f;
        }
    }

    public static void Postfix(CartoonShake __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
