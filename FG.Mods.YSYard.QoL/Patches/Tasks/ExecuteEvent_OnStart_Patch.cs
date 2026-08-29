using FG.Mods.YSYard.QoL.Services;
using HarmonyLib;
using Plot;

namespace FG.Mods.YSYard.QoL.Patches.Tasks;

[HarmonyPatch(typeof(ExecuteEvent), nameof(ExecuteEvent.lzd))]
public static class ExecuteEvent_OnStart_Patch
{
    public static void Postfix(ExecuteEvent __instance)
    {
        //ExLevelManager.OnTaskStart(__instance.ID);
        ExLevelManager.OnTaskStart(__instance.bhlx);
    }
}
