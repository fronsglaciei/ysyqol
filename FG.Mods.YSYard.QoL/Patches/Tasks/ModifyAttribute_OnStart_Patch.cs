using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(ModifyAttribute), nameof(ModifyAttribute.lzd))]
public static class ModifyAttribute_OnStart_Patch
{
    public static void Postfix(ModifyAttribute __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
